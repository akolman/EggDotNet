using System;

namespace EggDotNet.InternalExtensions
{
#if NETSTANDARD2_0
    internal static class ArrayExtensions
    {
        public static T[] Slice<T>(this T[] buffer, int offset, int count)
        {
            var rtn = new T[count];
            Array.Copy(buffer, offset, rtn, 0, count);
            return rtn;
        }
    }
#endif
}
