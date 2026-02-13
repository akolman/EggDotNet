using System;
using System.IO;

namespace EggDotNet.Compression.AZO
{
	internal sealed class AZOStream : Stream
	{
		private readonly Stream compressedStream;
		private readonly AZODecoder decode;
		private readonly DecompressedBufferCache decompBuffer;
		private readonly long usize;
		private long leftToRead;
		
		public AZOStream(long uncompressedSize, AZODecoder decoder, Stream inputStream) 
		{ 
			decode = decoder;
			compressedStream = inputStream;
			usize = leftToRead = uncompressedSize;
			decompBuffer = new DecompressedBufferCache();
		}

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => usize;

		public override long Position { get => usize - leftToRead; set => throw new InvalidOperationException(); }

		public override void Flush()
		{
			
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (leftToRead > 0)
			{
				var decompCount = decode.Decompress(compressedStream, decompBuffer, count);
				leftToRead -= decompCount;
			}

			if (decompBuffer.IsEmpty) return 0;

			var readCount = decompBuffer.Pop(count, buffer);

			return readCount;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException();
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException($"Cannot set length on {nameof(AZOStream)}");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException($"Cannot write to {nameof(AZOStream)}");
		}
	}
}
