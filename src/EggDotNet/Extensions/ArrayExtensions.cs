using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Extensions
{
	internal static class ArrayExtensions
	{
#if !NETSTANDARD2_1_OR_GREATER

		public static void Fill<T>(this T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] = value;
		}
#endif
	}
}
