using System.IO;

namespace EggDotNet.Compression
{
	internal class StoreCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			return subStream;
		}
	}
}
