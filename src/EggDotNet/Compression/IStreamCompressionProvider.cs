using EggDotNet.SpecialStreams;
using System.IO;

namespace EggDotNet.Compression
{
	internal interface IStreamCompressionProvider
	{
		Stream GetDecompressStream(Stream stream);
	}
}
