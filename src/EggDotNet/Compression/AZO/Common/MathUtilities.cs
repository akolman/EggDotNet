using System;

namespace EggDotNet.Compression.AZO.Common
{
	internal class MathUtilities
	{
		public static double Log2(uint x)
		{
			return Math.Log(x) / Math.Log(2);
		}
	}
}
