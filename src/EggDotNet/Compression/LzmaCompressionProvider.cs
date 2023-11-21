using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression
{
	internal sealed class LzmaCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream stream)
		{
			throw new NotImplementedException("LZMA not implemented");
		}
	}
}
