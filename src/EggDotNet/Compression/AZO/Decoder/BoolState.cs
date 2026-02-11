using EggDotNet.Compression.AZO.Common;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class BoolState
	{
		private readonly int N;
		private readonly uint ArrayN;
		private readonly int TotalBit;
		private readonly uint TotalCount;
		private readonly uint[] prob;
		private uint state;

		public BoolState(int n = AZOOption.N) 
		{
			N = n;
			ArrayN = 1U << N;
			TotalBit = 12;
			TotalCount = 1U << TotalBit;
			prob = new uint[ArrayN];
			for(uint i=0; i < ArrayN; ++i)
			{
				prob[i] = TotalCount / 2;
			}
		}

		public bool Code(EntropyCode entropy)
		{
			bool b = entropy.Code(GetProb(), TotalBit);
			Update(b);
			return b;
		}

		public uint GetProb()
		{
			return prob[state];
		}

		private void Update(bool b)
		{
			int ShiftBit = TotalBit - 6;
			ref uint p = ref prob[state];

			if (!b)
			{
				p += (TotalCount - p) >> ShiftBit;
			}
			else
			{
				p -= p >> ShiftBit;
			}
			uint a = (uint)(b ? 1 : 0);
			state = ((state << 1) & (ArrayN - 1)) | a;
		}
	}
}
