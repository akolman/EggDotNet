using EggDotNet.Encryption.Lea.Imp;
using System;
using System.IO;
using System.Security.Cryptography;

#pragma warning disable CA5350, CS0414

namespace EggDotNet.Encryption.Lea
{
	internal sealed class LeaStream : Stream
	{
		private bool _disposed;
		private bool _finalBlock;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override long Length => throw new NotImplementedException();

		public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		private Stream _stream;
		private ICryptoTransform _crypto;
		private HMACSHA1 _mac;
		private readonly byte[] _expectedMac;

		public LeaStream(Stream stream, ICryptoTransform cryptoTransform, byte[] macIv = null, byte[] expectedMac = null)
		{
			_stream = stream;
			_crypto = cryptoTransform;
			_expectedMac = expectedMac;
			if (macIv != null)
			{
				_mac = new HMACSHA1(macIv);
			}
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_finalBlock)
				return 0;

			var readBuf = new byte[count];
			var readLen = _stream.Read(readBuf, offset, count);
			if (readLen <= LeaCryptoTransform.BLOCK_SIZE_BYTES)
			{
				_mac?.TransformFinalBlock(readBuf, 0, readLen);
				_crypto.TransformFinalBlock(buffer, 0, readLen);
				_finalBlock = true;
				VerifyMac();
			}
			else
			{
				_mac?.TransformBlock(readBuf, 0, readLen, null, 0);
				for(var i=0; i <= count - LeaCryptoTransform.BLOCK_SIZE_BYTES; i+= LeaCryptoTransform.BLOCK_SIZE_BYTES)
				{
					_crypto.TransformBlock(readBuf, i, readLen, buffer, i);
				}
			}
			return readLen;
		}

		private void VerifyMac()
		{
			if (_mac == null || _expectedMac == null)
				return;

			byte[] computed = new byte[10];
			Array.Copy(_mac.Hash, 0, computed, 0, 10);

			if (computed.Length != _expectedMac.Length)
				throw new InvalidDataException("The MAC does not match.");

			int diff = 0;
			for (int i = 0; i < computed.Length; i++)
				diff |= computed[i] ^ _expectedMac[i];

			if (diff != 0)
				throw new InvalidDataException("The MAC does not match.");
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
					_mac?.Dispose();
					_stream = null;
					_crypto = null;
					_mac = null;
				}
				_disposed = true;
			}
			//base.Dispose(); //don't call this
		}
	}
}
