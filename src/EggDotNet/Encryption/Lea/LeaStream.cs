using EggDotNet.Encryption.Lea.Imp;
using System;
using System.IO;
using System.Security.Cryptography;

namespace EggDotNet.Encryption.Lea
{
	internal sealed class LeaStream : Stream
	{
		private bool _disposed;

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
			if (readLen <= LeaCryptoTransform.BLOCK_SIZE_BYTES)
			{
				_crypto.TransformFinalBlock(buffer, 0, readLen);
			}
			else
			{
				for(var i=0; i <= count - LeaCryptoTransform.BLOCK_SIZE_BYTES; i+= LeaCryptoTransform.BLOCK_SIZE_BYTES)
				{
					_crypto.TransformBlock(readBuf, i, readLen, buffer, i);
				}
			}
			return readLen;
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

#pragma warning disable CA2215
		protected override void Dispose(bool disposing)
#pragma warning restore CA2215
		{
			if (disposing)
			{
				if (!_disposed)
				{
					_stream.Dispose();
					_crypto.Dispose();
					_stream = null;
					_crypto = null;
				}
				_disposed = true;
			}
			//base.Dispose(); //don't call this
		}
	}
}
