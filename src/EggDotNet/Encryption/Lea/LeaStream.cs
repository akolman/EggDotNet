using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EggDotNet.Encryption.Lea
{
	internal class LeaStream : System.IO.Stream
	{
		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => throw new NotImplementedException();

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private Stream _stream;
		private ICryptoTransform _crypto;

		public LeaStream(Stream stream, ICryptoTransform cryptoTransform)
		{
			_stream = stream;
			_crypto = cryptoTransform;
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var readBuf = new byte[count];
			var readLen = _stream.Read(readBuf, offset, count);
			if (readLen < 16)
			{
				_crypto.TransformBlock(readBuf, 0, readLen, buffer, 0);
			}


			return readLen;
			//_crypto.TransformBlock
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
