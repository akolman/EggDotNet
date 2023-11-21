using EggDotNet.Compression.Bzip2;
using EggDotNet.SpecialStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression
{
	internal class BZip2CompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			return new BZip2InputStream(subStream);
		}
	}
}
