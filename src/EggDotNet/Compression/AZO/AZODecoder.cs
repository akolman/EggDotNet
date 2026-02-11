using EggDotNet.Compression.AZO.Decoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression.AZO
{
	internal class AZODecoder
	{
		private readonly MainCode mainCoder;

		public AZODecoder()
		{
			mainCoder = new MainCode();
		}

		public int Decompress(Stream stream, DecompressedBufferCache outBuffer, int readSize)
		{
			return mainCoder.Code(stream, outBuffer, readSize);
		}

		/*
		public int Decompress(byte[] inbuf, int inAvailable, byte[] outbuf, int outAvailable)
		{
			return mainCoder.Code(inbuf, inAvailable, outbuf, outAvailable);
		}*/
	}
}
