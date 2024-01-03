using System.IO;

namespace EggDotNet.Extensions
{
#if NETSTANDARD2_0
	internal static class StreamExtensions
	{
		public static int Read(this Stream stream, byte[] buffer)
		{
			return stream.Read(buffer, 0, buffer.Length);
		}
	}
#endif
}
