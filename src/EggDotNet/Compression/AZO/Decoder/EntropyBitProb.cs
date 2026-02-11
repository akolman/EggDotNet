using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class EntropyBitProb
	{
		private const int TotalBit = 10;
		private const uint TotalCount = 1 << TotalBit;

		private readonly int BitN;
		private readonly uint ArrayN;
		private readonly uint[] prob;

		public EntropyBitProb(uint N)
		{
			BitN = (int)MathUtilities.Log2(N - 1) + 1;
			ArrayN = (uint)(1 << (int)BitN);
			prob = new uint[ArrayN];
			for(var i=0; i<prob.Length; i++)
			{
				prob[i] = TotalCount / 2;
			}
		}

		public uint Code(EntropyCode entropy)
		{
			uint value = 0;
			uint pre = 1;

			for(var i=BitN - 1; i >= 0; --i)
			{
				var v = entropy.Code(GetProb(pre), TotalBit);
				if (v) value |= (uint)((int)1 << (int)i);

				var bv = (v ? 1 : 0);

				pre = (uint)(((int)pre << 1) | bv); 
			}

			Update(value);
			return value;
		}

		private uint GetProb(uint pre)
		{
			return prob[pre];
		}

		public void Update(uint value)
		{
			uint pre = 1;
			for(var i=BitN - 1; i >= 0; --i)
			{
				uint v = (value >> (int)i) & 1;
				IncreProb(pre, v > 0);
				pre = (pre << 1) | v;
			}
		}

		private void IncreProb(uint pre, bool value)
		{
			var shiftBit = TotalBit - 6;
			ref uint p = ref prob[pre];
			if (!value)
			{
				p += (TotalCount - p) >> (int)shiftBit;
			}
			else
			{
				p -= p >> (int)shiftBit;

			}
		}

		public int Compare(EntropyBitProb other, uint value)
		{
			uint prob1 = 1;
			uint prob2 = 1;

			uint pre = 1;
			for(int i=BitN - 1; i >= 0; --i)
			{
				uint p1 = this.GetProb(pre);
				uint p2 = other.GetProb(pre);
				bool v = ((value >> i) & 1) > 0;

				var t = ((prob1 | prob2) & (((1u << TotalBit) - 1) << (32 - TotalBit)));

				if (t != 0)
				{
					prob1 >>= TotalBit;
					prob2 >>= TotalBit;
				}

				prob1 *= v ? TotalCount - p1 : p1;
				prob2 *= v ? TotalCount - p2 : p2;

				pre = (pre << 1) | (v ? 1u : 0u);
			}

			if (prob1 > prob2) return 1;
			else if (prob1 < prob2) return -1;
			else return 0;
		}
	}
}
