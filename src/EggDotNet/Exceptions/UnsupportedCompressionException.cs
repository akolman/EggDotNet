using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an exception thrown when an unknown compression method is used.
	/// </summary>
	public class UnsupportedCompressionException : Exception
	{
		internal UnsupportedCompressionException(string compressionType)
			: base($"Compression type {compressionType} is unsupported")
		{
		}
	}
}
