using EggDotNet.Encryption.Lea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EggDotNet.Encryption.Lea.Imp;
using EggDotNet.Format.Alz;
using System.Linq;
using System.Security.Cryptography;

namespace EggDotNet.Encryption
{
	internal class LeaStreamDecryptionProvider : IStreamDecryptionProvider
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
			var lea = new Lea.Imp.Lea(256, password, _header.Take(16).ToArray());
			_cryptoTransform = lea.CreateDecryptor("CTR", lea.Key, new byte[16]);
			return true;
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			var st = new LeaStream(stream, _cryptoTransform);
			return st;
		}
	}
}
