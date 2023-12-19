using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an error thrown when an unknown compression method is used.
	/// </summary>
	public sealed class UnsupportedCompressionException : Exception
	{
		internal UnsupportedCompressionException(string compressionType)
			: base($"Compression type {compressionType} is unsupported")
		{
		}
	}
}
