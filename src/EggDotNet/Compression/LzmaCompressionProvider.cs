using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression
{
	internal class LzmaCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			throw new NotImplementedException("LZMA not implemented");
		}
	}
}
