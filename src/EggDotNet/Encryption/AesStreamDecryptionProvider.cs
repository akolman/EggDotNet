using EggDotNet.Encryption.Aes;
using System.IO;
using System.Linq;

namespace EggDotNet.Encryption
{
	internal sealed class AesStreamDecryptionProvider : IStreamDecryptionProvider
	{
		private readonly byte[] _footer;
		private readonly int _bits;
		private readonly byte[] _header;
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
			return new EggAesCipherStream(stream, _crypto, stream.Length, CryptoMode.Decrypt, _footer);
		}
	}
}
