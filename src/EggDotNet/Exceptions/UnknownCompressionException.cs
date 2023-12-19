using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an error thrown when an unknown compression method is used.
	/// </summary>
	public sealed class UnknownCompressionException : Exception
	{
		internal UnknownCompressionException(byte compressionMethodValue)
			: base($"Compression type {compressionMethodValue} is unknown")
		{
		}
	}
}
