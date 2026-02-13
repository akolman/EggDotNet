using System;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an Exception thrown when decompression of data fails.
	/// </summary>
	public sealed class DecompressionDataException : Exception
	{
		internal DecompressionDataException(string message = "An error was encountered during decompression of the stream")
			: base(message) { }
	}
}
