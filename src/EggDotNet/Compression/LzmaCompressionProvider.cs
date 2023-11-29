using EggDotNet.Compression.LZMA;
using System.IO;

namespace EggDotNet.Compression
{

	internal sealed class LzmaCompressionProvider : IStreamCompressionProvider
	{
		private readonly long _compSize;
		private readonly long _uncompSize;

		public LzmaCompressionProvider(long compressedSize, long uncompressedSize)
		{ 
			_compSize = compressedSize;
			_uncompSize = uncompressedSize;
		}

		public Stream GetDecompressStream(Stream stream)
		{
			stream.Seek(4, SeekOrigin.Begin);
			byte[] props = new byte[5];
			stream.Read(props, 0, 5);
			return new LzmaStream(props, stream, _compSize - 9, _uncompSize);
		}
	}
}
