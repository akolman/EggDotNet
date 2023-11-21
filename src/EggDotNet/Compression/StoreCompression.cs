using EggDotNet.SpecialStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Compression
{
	internal class StoreCompression : IStreamCompression
	{
		public Stream GetDecompressStream(Stream subStream)
		{
			return subStream;
		}
	}
}
