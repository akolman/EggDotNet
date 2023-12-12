using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static EggDotNet.Compression.LZMA.Base;

namespace EggDotNet.Compression.AZO.Implementation.Decoder
{
	internal class BoolState
	{
		private const uint N = 8;
		private const uint ARRAY_N = 1 << 8;
		private const uint TOTAL_BIT = 12;
		private const uint TOTAL_COUNT = 1 << 12;

		private uint[] prob = new uint[ARRAY_N];
		private uint _state;

		public void Update(bool b)
		{
			uint SHIFT_BIT = TOTAL_BIT - 6;
			uint p = prob[_state];

			if (b)
			{
				p += (TOTAL_COUNT - p) >> (int)SHIFT_BIT;
			}
			else
			{
				p -= p >> (int)SHIFT_BIT;
			}

			_state = ((_state << 1) & (ARRAY_N - 1)) | (b ? 1u : 0);
		}

		private uint GetProb()
		{
			return prob[_state];
		}

		private uint TotalBit()
		{
			return TOTAL_BIT;
		}
	}
}
