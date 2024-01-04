using EggDotNet.Encryption.Zip;
using System.IO;

#pragma warning disable CA1001

namespace EggDotNet.Encryption
{
	internal sealed class ZipStreamDecryptionProvider : IStreamDecryptionProvider
	{
		private readonly byte[] _vdata;
		private readonly byte[] _crc;
		private ZipDecryptStream _decrypt;

		public ZipStreamDecryptionProvider(byte[] vdata, byte[] crc)
		{
			_vdata = vdata;
			_crc = crc;
		}

		public bool AttachAndValidatePassword(string password)
		{
			_decrypt = new ZipDecryptStream(password, _vdata, _crc);
			if (_decrypt.PasswordValid)
			{
				return true;
			}
			else
			{
				_decrypt.Dispose();
				_decrypt = null;
				return false;
			}
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			_decrypt.AttachStream(stream);
			return _decrypt;
		}


	}
}
