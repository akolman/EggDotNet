using EggDotNet.Encryption.Zip;
using System.IO;

#pragma warning disable CA1001

namespace EggDotNet.Encryption
{
	internal sealed class ZipStreamDecryptionProvider : IStreamDecryptionProvider
	{
		public bool PasswordValid => _decrypt.PasswordValid;

		private readonly byte[] _vdata;
		private readonly byte[] _crc;
		private string _password;
		private readonly ZipDecryptStream _decrypt;

		public ZipStreamDecryptionProvider(byte[] vdata, byte[] crc, string password)
		{
			_vdata = vdata;
			_crc = crc;
			_password = password;
			_decrypt = new ZipDecryptStream(password, _vdata, _crc);
		}

		public bool AttachAndValidatePassword(string password)
		{
			throw new System.NotImplementedException();
		}

		public Stream GetDecryptionStream(Stream stream)
		{
			_decrypt.AttachStream(stream);
			return _decrypt;
		}


	}
}
