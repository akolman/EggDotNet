using System;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class HistoryList
	{
		private readonly uint N;
		private readonly uint[] rep;
		private readonly BoolState state;
		private readonly EntropyBitProb prob;

		public HistoryList(uint n) : this(0, n)
		{
		}

		public HistoryList(uint init, uint n)
		{
			N = n;
			rep = new uint[N];
			for(uint i=0; i < N; ++i)
			{
				rep[i] = init + i;
			}
			state = new BoolState();
			prob = new EntropyBitProb(N);
		}

		public void Add(ref uint value)
		{
			Array.Copy(rep, 0, rep, 1, N - 1); //AIOK?
			rep[0] = value;
		}

		public void Add(ref uint value, uint delIdx)
		{
			Array.Copy(rep, 0, rep, 1, delIdx);//AIOK?
			rep[0] = value;
		}

		public bool Code(EntropyCode entropy, ref uint value)
		{
			if (state.Code(entropy))
			{
				uint idx = prob.Code(entropy);
				value = rep[idx];
				Add(ref value, idx);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
