using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal sealed class LengthCode
	{
		private readonly PredictProb prob;

		public LengthCode()
		{
			prob = new PredictProb(AZOOption.MATCH_DIST_CODE_SIZE, AZOOption.MATCH_LENGTH_CODE_SIZE, AZOOption.LENGTHCODE_PREDICT_SHIFT);
		}

		public uint Code(EntropyCode entropy, uint distCode)
		{
			var lenCode = prob.Code(entropy, distCode);
			var len = MatchCodeTable.MATCH_LENGTH_CODE_TABLE[lenCode];
			if (lenCode <= MatchCodeTable.MATCH_LENGTH_EXTRABIT_TABLE.Length - 1)
			{
				var extraBits = entropy.Code(MatchCodeTable.MATCH_LENGTH_EXTRABIT_TABLE[lenCode]);
				len += extraBits;
			}
			return len;
		}
	}
}
