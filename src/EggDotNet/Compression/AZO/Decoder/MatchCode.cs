using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class MatchCode
	{
		private readonly DictionaryTable dictTable;
		private readonly DistanceCode distProb;
		private readonly LengthCode lenProb;

		public MatchCode(byte[] buf, uint bufSize)
		{
			dictTable = new DictionaryTable(buf, bufSize);
			distProb = new DistanceCode();
			lenProb = new LengthCode();
		}

		public void Code(EntropyCode entropy, uint pos, ref uint dist, ref uint length)
		{
			uint dictPos = 0;
			if (!dictTable.Code(entropy, ref dictPos, ref length))
			{
				dist = distProb.Code(entropy);
				length = lenProb.Code(entropy, MatchCodeTable.GetMatchDistCode(dist));

				dictTable.Add(pos, length);
			}
			else
			{
				dist = pos - dictPos;
			}
		}
	}
}
