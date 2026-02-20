using System;
using System.IO;
using System.Security.Cryptography;

#pragma warning disable

namespace EggDotNet.Encryption.Aes
{
	internal class EggAesCipherStream : Stream
	{
		private readonly EggAesCrypto _params;
		private readonly System.IO.Stream _s;
		private readonly CryptoMode _mode;
		private int _nonce;
		private bool _finalBlock;

		internal HMACSHA1 _mac;

		internal RijndaelManaged _aesCipher;
		internal ICryptoTransform _xform;

		private const int BLOCK_SIZE_IN_BYTES = 16;

		private readonly byte[] counter = new byte[BLOCK_SIZE_IN_BYTES];
		private byte[] counterOut = new byte[BLOCK_SIZE_IN_BYTES];

		private readonly long _length;
		private long _totalBytesXferred;
		private readonly byte[] _PendingWriteBlock;
		private int _pendingCount;
		private readonly byte[] _iobuf;

		internal EggAesCipherStream(System.IO.Stream s, EggAesCrypto cryptoParams, long length, CryptoMode mode)
			: base()
		{
			_params = cryptoParams;
			_s = s;
			_mode = mode;
			_nonce = 1;
			_length = length;

			if (_params == null)
				throw new System.Exception("Supply a password to use AES encryption.");

			int keySizeInBits = _params.KeyBytes.Length * 8;
			if (keySizeInBits != 256 && keySizeInBits != 128 && keySizeInBits != 192)
				throw new ArgumentOutOfRangeException("keysize",
													  "size of key must be 128, 192, or 256");

			_mac = new HMACSHA1(_params.MacIv);

			_aesCipher = new RijndaelManaged
			{
				BlockSize = 128,
				KeySize = keySizeInBits,  // 128, 192, 256
				Mode = CipherMode.ECB,
				Padding = PaddingMode.None
			};

			byte[] iv = new byte[BLOCK_SIZE_IN_BYTES]; // all zeroes

			_xform = _aesCipher.CreateEncryptor(_params.KeyBytes, iv);

			if (_mode == CryptoMode.Encrypt)
			{
				_iobuf = new byte[2048];
				_PendingWriteBlock = new byte[BLOCK_SIZE_IN_BYTES];
			}
		}

		private void XorInPlace(byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = (byte)(counterOut[i] ^ buffer[offset + i]);
			}
		}

		private void WriteTransformOneBlock(byte[] buffer, int offset)
		{
			System.Array.Copy(BitConverter.GetBytes(_nonce++), 0, counter, 0, 4);
			_xform.TransformBlock(counter,
								  0,
								  BLOCK_SIZE_IN_BYTES,
								  counterOut,
								  0);
			XorInPlace(buffer, offset, BLOCK_SIZE_IN_BYTES);
			_mac.TransformBlock(buffer, offset, BLOCK_SIZE_IN_BYTES, null, 0);
		}

		private void WriteTransformBlocks(byte[] buffer, int offset, int count)
		{
			int posn = offset;
			int last = count + offset;

			while (posn < buffer.Length && posn < last)
			{
				WriteTransformOneBlock(buffer, posn);
				posn += BLOCK_SIZE_IN_BYTES;
			}
		}

		private void WriteTransformFinalBlock()
		{
			if (_pendingCount == 0)
				throw new InvalidOperationException("No bytes available.");

			if (_finalBlock)
				throw new InvalidOperationException("The final block has already been transformed.");

			System.Array.Copy(BitConverter.GetBytes(_nonce++), 0, counter, 0, 4);
			counterOut = _xform.TransformFinalBlock(counter,
													0,
													BLOCK_SIZE_IN_BYTES);
			XorInPlace(_PendingWriteBlock, 0, _pendingCount);
			_mac.TransformFinalBlock(_PendingWriteBlock, 0, _pendingCount);
			_finalBlock = true;
		}

		private int ReadTransformOneBlock(byte[] buffer, int offset, int last)
		{
			if (_finalBlock)
				throw new NotSupportedException();

			int bytesRemaining = last - offset;
			int bytesToRead = (bytesRemaining > BLOCK_SIZE_IN_BYTES)
				? BLOCK_SIZE_IN_BYTES
				: bytesRemaining;

			// update the counter
			System.Array.Copy(BitConverter.GetBytes(_nonce++), 0, counter, 0, 4);

			// Determine if this is the final block
			if ((bytesToRead == bytesRemaining) &&
				(_length > 0) &&
				(_totalBytesXferred + last == _length))
			{
				_mac.TransformFinalBlock(buffer, offset, bytesToRead);
				counterOut = _xform.TransformFinalBlock(counter,
														0,
														BLOCK_SIZE_IN_BYTES);
				_finalBlock = true;
			}
			else
			{
				_mac.TransformBlock(buffer, offset, bytesToRead, null, 0);
				_xform.TransformBlock(counter,
									  0, // offset
									  BLOCK_SIZE_IN_BYTES,
									  counterOut,
									  0);  // offset
			}

			XorInPlace(buffer, offset, bytesToRead);
			return bytesToRead;
		}

		private void ReadTransformBlocks(byte[] buffer, int offset, int count)
		{
			int posn = offset;
			int last = count + offset;

			while (posn < buffer.Length && posn < last)
			{
				int n = ReadTransformOneBlock(buffer, posn, last);
				posn += n;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_mode == CryptoMode.Encrypt)
				throw new NotSupportedException();

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset",
													  "Must not be less than zero.");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count",
													  "Must not be less than zero.");

			if (buffer.Length < offset + count)
				throw new ArgumentException("The buffer is too small");

			int bytesToRead = count;

			if (_totalBytesXferred >= _length)
			{
				return 0; // EOF
			}

			long bytesRemaining = _length - _totalBytesXferred;
			if (bytesRemaining < count) bytesToRead = (int)bytesRemaining;

			int n = _s.Read(buffer, offset, bytesToRead);

			ReadTransformBlocks(buffer, offset, bytesToRead);

			_totalBytesXferred += n;
			return n;
		}

		/// <summary>
		/// Returns the final HMAC-SHA1-80 for the data that was encrypted.
		/// </summary>
		public byte[] FinalAuthentication
		{
			get
			{
				if (!_finalBlock)
				{
					// special-case zero-byte files
					if (_totalBytesXferred != 0)
						throw new System.Exception("The final hash has not been computed.");

					// Must call ComputeHash on an empty byte array when no data
					// has run through the MAC.

					byte[] b = { };
					_mac.ComputeHash(b);
					// fall through
				}
				byte[] macBytes10 = new byte[10];
				System.Array.Copy(_mac.Hash, 0, macBytes10, 0, 10);
				return macBytes10;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException($"Cannot write to {nameof(EggAesCipherStream)}");
		}

		/// <summary>
		///   Close the stream.
		/// </summary>
		public override void Close()
		{
			// In the degenerate case, no bytes have been written to the
			// stream at all.  Need to check here, and NOT emit the
			// final block if Write has not been called.
			if (_pendingCount > 0)
			{
				WriteTransformFinalBlock();
				_s.Write(_PendingWriteBlock, 0, _pendingCount);
				_totalBytesXferred += _pendingCount;
				_pendingCount = 0;
			}
			_s.Close();
		}

		/// <summary>
		/// Returns true if the stream can be read.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				if (_mode != CryptoMode.Decrypt) return false;
				return true;
			}
		}

		/// <summary>
		/// Always returns false.
		/// </summary>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Returns true if the CryptoMode is Encrypt.
		/// </summary>
		public override bool CanWrite
		{
			get { return (_mode == CryptoMode.Encrypt); }
		}

		/// <summary>
		/// Flush the content in the stream.
		/// </summary>
		public override void Flush()
		{
			_s.Flush();
		}

		/// <summary>
		/// Getting this property throws a NotImplementedException.
		/// </summary>
		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Getting or Setting this property throws a NotImplementedException.
		/// </summary>
		public override long Position
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// This method throws a NotImplementedException.
		/// </summary>
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This method throws a NotImplementedException.
		/// </summary>
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

#pragma warning disable IDE0052 // Remove unread private members
		private readonly object _outputLock = new Object();
#pragma warning restore IDE0052 // Remove unread private members
	}
}
