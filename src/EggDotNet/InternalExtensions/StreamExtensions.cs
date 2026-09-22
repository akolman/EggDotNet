using System.IO;

namespace EggDotNet.InternalExtensions
{
#if LEGACY_DOTNET
    internal static class StreamExtensions
    {
        public static int Read(this Stream stream, byte[] buffer)
        {
            return stream.Read(buffer, 0, buffer.Length);
        }
    }
#endif
}
