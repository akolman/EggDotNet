using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal sealed class AlphaCode
	{
		private readonly PredictProb alphaProb;

		public AlphaCode()
		{
			alphaProb = new PredictProb(AZOOption.ALPHA_SIZE, AZOOption.ALPHA_SIZE, AZOOption.ALPHACODE_PREDICT_SHIFT);
		}

		public byte Code(EntropyCode entropy, uint pos, byte pre)
		{
			_ = pos;
			return (byte)alphaProb.Code(entropy, pre);
		}

	}
}
