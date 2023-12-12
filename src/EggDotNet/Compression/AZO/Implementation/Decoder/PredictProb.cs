using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Compression.AZO.Implementation.Decoder
{
	internal class PredictProb
	{
		private uint _key;
		private uint _n;
		private uint _shift;

		public PredictProb(uint key, uint n, uint shift)
		{
			_key = key;
			_n = n;
			_shift = shift;
		}

		public uint Code(EntropyCode ec, uint pre)
		{

		}
	}
}
