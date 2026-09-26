using EggDotNet.Compression.AZO;
using System.IO;

namespace EggDotNet.Compression
{
	internal sealed class AzoCompressionProvider : IStreamCompressionProvider
	{
		private readonly AZODecoder decoder;
		private readonly long usize;

		public AzoCompressionProvider(long uncompressedSize)
		{
			decoder = new AZODecoder();
			usize = uncompressedSize;
		}

		public Stream GetDecompressStream(Stream stream) => new AZOStream(usize, decoder, stream);
	}
}
