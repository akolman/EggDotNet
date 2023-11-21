using System.IO.Compression;
using System.IO;
using EggDotNet.SpecialStreams;

namespace EggDotNet.Compression
{
	internal class DeflateCompression : IStreamCompression
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			var st = new DeflateStream(subStream, CompressionMode.Decompress, true);
			return st;
		}
	}
}
