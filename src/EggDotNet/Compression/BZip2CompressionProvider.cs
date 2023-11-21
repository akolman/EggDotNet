using EggDotNet.Compression.Bzip2;
using System.IO;

namespace EggDotNet.Compression
{
	internal sealed class BZip2CompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream stream)
		{
			return new BZip2InputStream(stream);
		}
	}
}
