using System.IO;

namespace EggDotNet.Compression
{
	/// <summary>
	/// Internal class used to create producers for specific decompression streams.
	/// </summary>
	internal interface IStreamCompressionProvider
	{
		Stream GetDecompressStream(Stream stream);
	}
}
