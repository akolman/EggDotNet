using EggDotNet.Encryption.Lea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace EggDotNet.Encryption
{
	internal sealed class LeaStreamDecryptionProvider : IStreamDecryptionProvider
	{
#pragma warning disable IDE0052 // Remove unread private members
		private readonly byte[] _footer;
#pragma warning restore IDE0052 // Remove unread private members
		private int _bits;
		private byte[] _header;
		ICryptoTransform _cryptoTransform;

		public LeaStreamDecryptionProvider(int bits, byte[] header, byte[] footer)
		{
			_footer = footer;
			_bits = bits;
			_header = header;
		}

		public bool AttachAndValidatePassword(string password)
		{
			var lea = new Lea.Imp.Lea(_bits, password, _header);
			if (lea.PasswordValid)
			{
				_cryptoTransform = lea.CreateDecryptor(lea.Key, new byte[16]);
				return true;
			}
			lea.Dispose();
			return false;
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			var st = new LeaStream(stream, _cryptoTransform);
			return st;
		}
	}
}
