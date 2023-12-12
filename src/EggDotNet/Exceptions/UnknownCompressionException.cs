using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when an unknown compression method is used.
	/// </summary>
	public class UnknownCompressionException : Exception
	{
		internal UnknownCompressionException(byte compressionMethodValue)
			: base($"Compression type {compressionMethodValue} is unknown")
		{
		}
	}
}
