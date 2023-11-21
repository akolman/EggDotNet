using EggDotNet.Encryption.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Encryption
{
	internal class ZipStreamDecryptionProvider : IStreamDecryptionProvider
	{
		public bool PasswordValid => true;

		private readonly byte[] _vdata;
		private readonly byte[] _crc;
		private readonly string _password;

		public ZipStreamDecryptionProvider(byte[] vdata, byte[] crc, string password)
		{
			_vdata = vdata;
			_crc = crc;
			_password = password;
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			return new ZipDecryptStream(stream, _password, _vdata, _crc);
		}
	}
}
