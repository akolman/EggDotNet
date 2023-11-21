using System.IO;

namespace EggDotNet.Encryption
{
	internal interface IStreamDecryptionProvider
	{
		bool PasswordValid { get; }

		Stream GetDecryptionStream(Stream stream);
	}
}
