using EggDotNet.Encryption.Aes;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

#pragma warning disable 

namespace EggDotNet.Encryption
{
	internal class AesStreamDecryptionProvider : IStreamDecryptionProvider
	{
#pragma warning disable IDE0052 // Remove unread private members
		private readonly byte[] _footer;
#pragma warning restore IDE0052 // Remove unread private members
		private int _bits;
		private byte[] _header;
		private EggAesCrypto _crypto;
		public AesStreamDecryptionProvider(int bits, byte[] header, byte[] footer)
		{
			_footer = footer;
			_bits = bits;
			_header = header;
		}

		public bool PasswordValid => _crypto.PasswordValid;

		public bool AttachAndValidatePassword(string password)
		{
			if (256 == _bits)
			{
				_crypto = EggAesCrypto.ReadFromStream(password, _bits, _header.Take(16).ToArray(), _header.Skip(16).Take(2).ToArray());
			}
			else
			{
				_crypto = EggAesCrypto.ReadFromStream(password, _bits, _header.Take(8).ToArray(), _header.Skip(8).Take(2).ToArray());
			}

			return _crypto.PasswordValid;	
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			var decrypt = new EggAesCipherStream(stream, _crypto, stream.Length, CryptoMode.Decrypt);

			return decrypt;
		}
	}
}
