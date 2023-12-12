using EggDotNet.Compression.AZO.Implementation.Common;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETSTANDARD2_1_OR_GREATER
using EggDotNet.Extensions;
#endif

namespace EggDotNet.Compression.AZO.Implementation.Decoder
{
	internal class EntityBitProb
	{
		private const uint BIT_N = 8; //log2(ALPHA_SIZE)
		private const uint ARRAY_N = 256; //1<<BIT_N
		private const uint TOTAL_BIT = 10;
		private const uint TOTAL_COUNT = 1024; //1<<TOTAL_BIT

		private uint[] _prob = new uint[ARRAY_N];

		public EntityBitProb()
		{
			ArrayExtensions.Fill(new byte[], )
			Array.Fill()
		}
	}
}
