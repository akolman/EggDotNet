using System;

namespace EggDotNet.InternalExtensions
{
#if NETSTANDARD2_0
    internal static class BitConverterWrapper
    {
        public static int ToInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        public static short ToInt16(byte[] buffer)
        {
            return BitConverter.ToInt16(buffer, 0);
        }

        public static uint ToUInt32(byte[] buffer)
        {
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static long ToInt64(byte[] buffer)
        {
            return BitConverter.ToInt64(buffer, 0);
        }
    }
#endif
}
