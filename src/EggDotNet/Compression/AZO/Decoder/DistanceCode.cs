using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class DistanceCode
	{
		private readonly HistoryList history;
		private readonly EntropyBitProb prob;

		public DistanceCode()
		{
			history = new HistoryList(AZOOption.MATCH_MIN_DIST, AZOOption.DISTANCE_HISTORY_SIZE);
			prob = new EntropyBitProb(AZOOption.MATCH_DIST_CODE_SIZE);
		}

		public uint Code(EntropyCode entropy)
		{
			uint dist = 0;
			if (!history.Code(entropy, ref dist))
			{
				uint idxCode = prob.Code(entropy);
				dist = MatchCodeTable.MATCH_DIST_CODE_TABLE[idxCode];
				if (idxCode <= MatchCodeTable.MATCH_DIST_EXTRABIT_TABLE.Length - 1)
				{
					dist += entropy.Code(MatchCodeTable.MATCH_DIST_EXTRABIT_TABLE[idxCode]);
				}
				history.Add(ref dist);
			}

			return dist;
		}
	}
}
