using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when an unsupported encryption method was used.
	/// </summary>
	public sealed class UnsupportedEncryptionException : Exception
	{
		internal UnsupportedEncryptionException(string encryptionType)
			: base($"Encryption type {encryptionType} is unsupported")
		{
		}
	}
}
