using EggDotNet.SpecialStreams;
using System.IO;

namespace EggDotNet.Compression
{
	internal interface IStreamCompression
	{
		Stream GetDecompressStream(Stream subStream);
	}
}
