using System.IO;

namespace EggDotNet.Encryption
{
	internal interface IStreamDecryptionProvider
	{
		bool PasswordValid { get; }

		bool AttachAndValidatePassword(string password);

		Stream GetDecryptionStream(Stream stream);
	}
}
