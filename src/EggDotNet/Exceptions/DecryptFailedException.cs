using System;

namespace EggDotNet.Exceptions
{
	public sealed class DecryptFailedException : Exception
	{
		internal DecryptFailedException()
			: base("Decryption of entry failed")
		{ }
	}
}
