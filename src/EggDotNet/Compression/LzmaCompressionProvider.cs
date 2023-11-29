using EggDotNet.Compression.LZMA;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace EggDotNet.Compression
{

	internal sealed class LzmaCompressionProvider : IStreamCompressionProvider
	{
		private long _compSize;
		private long _uncompSize;

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
			var st = new LzmaStream(props, stream, _compSize - 9, _uncompSize);
			return st;

			/*


			var decomp = new LZMA.Decoder();
			decomp.SetDecoderProperties(props);

			var memStream = new MemoryStream();
	
			decomp.Code(stream, memStream, _compSize, _uncompSize, null);
			memStream.Seek(0, SeekOrigin.Begin);
			return memStream;*/
		}
	}
}
