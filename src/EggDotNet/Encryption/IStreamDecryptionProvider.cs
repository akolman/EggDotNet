using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Encryption
{
	internal interface IStreamDecryptionProvider
	{
		bool PasswordValid { get; }

		Stream GetDecryptionStream(Stream stream);
	}
}
