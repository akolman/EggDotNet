using System;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal sealed class PredictProb
	{
		private readonly uint Key;
		private readonly uint N;
		private readonly uint Shift;
		private readonly EntropyBitProb[] prob1;
		private readonly EntropyBitProb[] prob2;
		private readonly int[] lucky;

		public PredictProb(uint key, uint n, uint shift)
		{
			Key = key;
			N = n;
			Shift = shift;
			prob1 = new EntropyBitProb[Key];
			for (var i = 0; i < prob1.Length; i++) prob1[i] = new EntropyBitProb(N);
			prob2 = new EntropyBitProb[(int)Key >> (int)Shift];
			for (var i = 0; i < prob2.Length; i++) prob2[i] = new EntropyBitProb(N);
			lucky = new int[Key];
		}

		public uint Code(EntropyCode entropy, uint pre)
		{
			uint v;

			EntropyBitProb p1 = prob1[pre];
			EntropyBitProb p2 = prob2[pre >> (int)Shift];

			if (lucky[pre] >= 0)
			{
				v = p1.Code(entropy);
				p2.Update(v);
			}
			else
			{
				v = p2.Code(entropy);
				p1.Update(v);
			}

			var r = p1.Compare(p2, v);
			if (r > 0)
			{
				++lucky[pre];
			}
			else if(r < 0)
			{
				--lucky[pre];
			}

			return v;
		}

	}
}
