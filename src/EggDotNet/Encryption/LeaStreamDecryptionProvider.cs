using EggDotNet.Encryption.Lea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace EggDotNet.Encryption
{
	internal class LeaStreamDecryptionProvider : IStreamDecryptionProvider
	{
		private string password;
		private byte[] header;
		private byte[] footer;
		LeaDecryptionStream leastream;


		public LeaStreamDecryptionProvider(int bits, byte[] header, byte[] footer, string password)
		{ 
			leastream = new Lea.LeaDecryptionStream(password, header, footer); ;

		}

		public bool PasswordValid => true;

		public Stream GetDecryptionStream(Stream stream)
		{
			leastream.AttachStream(stream);
			return leastream;
		}
	}
}
