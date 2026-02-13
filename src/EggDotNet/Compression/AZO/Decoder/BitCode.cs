using System;

namespace EggDotNet.Compression.AZO.Decoder
{
	internal sealed class BitCode
	{
		private const uint TYPE_BIT_SIZE = sizeof(byte) * 8;
		private const byte TYPE_BIT_MASK = (1 << (int)TYPE_BIT_SIZE) - 1;

		private readonly byte[] inBuf;
		private readonly uint bufSize;
		private uint bufIdx;
		private uint remainBit;
		private uint readSize;

		public BitCode(byte[] buffer, uint size)
		{
			inBuf = buffer;
			bufSize = size*8;
			remainBit = TYPE_BIT_SIZE;
			readSize = 0;
			bufIdx = 0;
		}

		public bool Code(ref byte value, uint bitsize)
		{
			if (bufSize < readSize + bitsize)
			{
				return false;
			}

			readSize += bitsize;
			value = 0;

			if (remainBit <= bitsize)
			{
				bitsize -= remainBit;
				value = (byte)((inBuf[bufIdx++] & ((1 << (int)remainBit) - 1)) << (int)bitsize);
				remainBit = TYPE_BIT_SIZE;
			
				while(bitsize >= TYPE_BIT_SIZE)
				{
					bitsize -= TYPE_BIT_SIZE;
					value |= (byte)((inBuf[bufIdx++] & TYPE_BIT_MASK) << (int)bitsize);
				}
			}

			if (bitsize != 0)
			{
				remainBit -= bitsize;
				value |= (byte)((inBuf[bufIdx] >> (int)remainBit) & ((1 << (int)bitsize) - 1));
			}

			return true;
		}

		/*
		public bool Code(byte[] values, uint bitsize)
		{
			for(var i=0; i < bitsize/8; ++i)
			{
				if (!Code(ref values[i], TYPE_BIT_SIZE))
				{
					return false;
				}
			}

			throw new NotImplementedException();

			return false;
		}*/

		public bool Code(ref bool v)
		{
			if (bufSize == readSize)
			{
				return false;
			}

			++readSize;
			--remainBit;

			if (bufIdx >= inBuf.Length)
			{
				return false;
			}

			v = (inBuf[bufIdx] & (1U << (int)remainBit)) != 0;

			if (remainBit == 0)
			{
				++bufIdx;
				remainBit = TYPE_BIT_SIZE;
			}

			return true;
		}

		public uint GetReadSize() => readSize;
	}
}
