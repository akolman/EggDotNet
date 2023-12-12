using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exceptions
{
	/// <summary>
	/// Represents an error thrown when an unknown compression method is used.
	/// </summary>
	public class UnsupportedCompressionException : Exception
	{
		internal UnsupportedCompressionException(string compressionType)
			: base($"Compression type {compressionType} is unsupported")
		{
		}
	}
}
