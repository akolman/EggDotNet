using EggDotNet.Encryption.Aes;
using System.IO;
using System.Linq;

namespace EggDotNet.Encryption
{
	internal class AesStreamDecryptionProvider : IStreamDecryptionProvider
	{
#pragma warning disable IDE0052 // Remove unread private members
		private readonly byte[] _footer;
#pragma warning restore IDE0052 // Remove unread private members
		private readonly EggAesCrypto _crypto;
		public AesStreamDecryptionProvider(int bits, byte[] header, byte[] footer, string password)
		{
			_footer = footer;
			if (256 ==  bits)
			{
				_crypto = EggAesCrypto.ReadFromStream(password, bits, header.Take(16).ToArray(), header.Skip(16).Take(2).ToArray());
			}
			else
			{
				_crypto = EggAesCrypto.ReadFromStream(password, bits, header.Take(8).ToArray(), header.Skip(8).Take(2).ToArray());
			}

		}

		public bool PasswordValid => _crypto.PasswordValid;

		public Stream GetDecryptionStream(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			return new EggAesDecryptionStream(stream, _crypto, stream.Length, CryptoMode.Decrypt);
		}
	}
}
