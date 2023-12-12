using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when an unknown encryption method was used.
	/// </summary>
	public sealed class UnknownEncryptionException : Exception
	{
		internal UnknownEncryptionException(byte encryptionMethodValue)
			: base($"Encryption type {encryptionMethodValue} is unknown")
		{
		}
	}
}
