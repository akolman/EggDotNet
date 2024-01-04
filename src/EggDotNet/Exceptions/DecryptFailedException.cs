using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Exception indicating that decryption has failed.
	/// </summary>
	public sealed class DecryptFailedException : Exception
	{
		internal DecryptFailedException()
			: base("Decryption of entry failed")
		{ }
	}
}
