using System.IO;

namespace EggDotNet.Compression
{
	internal sealed class StoreCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream stream)
		{
			return stream;
		}
	}
}
