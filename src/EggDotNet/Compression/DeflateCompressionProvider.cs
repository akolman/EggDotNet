using System.IO.Compression;
using System.IO;

namespace EggDotNet.Compression
{
	internal sealed class DeflateCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream stream) 
			=> new DeflateStream(stream, CompressionMode.Decompress, true);
	}
}
