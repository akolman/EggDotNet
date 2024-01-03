using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Extensions
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
