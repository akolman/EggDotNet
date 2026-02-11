using EggDotNet.Compression.AZO.Common;
using System;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal class DictionaryTable
	{
		private const uint N = AZOOption.DICTIONARY_SIZE;

		private readonly Data[] data;
		private readonly byte[] buffer;
		private readonly uint bufSize;
		private readonly BoolState findState;
		private readonly SymbolCode prob;

		public DictionaryTable(byte[] buf, uint size)
		{
			data = new Data[N];
			for(uint i=0; i< N; ++i)
			{
				data[i].length = AZOOption.MATCH_MIN_LENGTH + i;
			}
			findState = new BoolState();
			prob = new SymbolCode(AZOOption.DICTIONARY_SIZE, AZOOption.DICTIONARY_HISTORY_SIZE);
			buffer = buf;
			bufSize = size;
		}

		public bool Code(EntropyCode entropy, ref uint pos, ref uint len)
		{
			if (findState.Code(entropy))
			{
				uint n = prob.Code(entropy);
				if (n >= N)
					return false;

				Get(n, ref pos, ref len);
				Update(pos, len, n);
				return true;
			}

			return false;
		}

		public void Add(uint pos, uint length)
		{
			Update(pos, length);
		}

		public void Get(uint idx, ref uint pos, ref uint len)
		{
			len = data[idx].length;
			pos = data[idx].position;
		}

		private void Update(uint pos, uint len)
		{
			Array.Copy(data, 0, data, 1, N - 1); //AIOK?
			data[0].length = len; 
			data[0].position = pos;
		}

		private void Update(uint pos, uint len, uint delIdx)
		{
			Array.Copy(data, 0, data, 1, delIdx);//AIOK?
			data[0].length = len;  
			data[0].position = pos;
		}

		internal struct Data
		{
			public uint position;
			public uint length;
		}
	}
}
