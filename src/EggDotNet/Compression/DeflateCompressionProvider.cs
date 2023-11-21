using System.IO.Compression;
using System.IO;
using EggDotNet.SpecialStreams;

namespace EggDotNet.Compression
{
	internal class DeflateCompressionProvider : IStreamCompressionProvider
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			var st = new DeflateStream(subStream, CompressionMode.Decompress, true);
			return st;
		}
	}
}
