using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Exceptions
{
	internal class DecompressionDataException : Exception
	{
		public DecompressionDataException(string message = "An error was encountered during decompression of the stream")
			: base(message) { }
	}
}
