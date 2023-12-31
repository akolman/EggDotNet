using System;
using System.Collections.Generic;
using System.Text;

namespace EggDotNet.Extensions
{
	internal static class Utilities
	{
		private const long EGG_MODDATE_EPOCH_TICKS = 504911232000000000;

		private const long ALZ_MODDATE_EPOCH_TICKS = 623695183570000000;

		public static DateTime FromEggTime(long timeVal)
		{
			return new DateTime(timeVal + EGG_MODDATE_EPOCH_TICKS);
		}

		public static DateTime FromAlzTime(long timeVal)
		{
			return new DateTime(timeVal * 10000000L + ALZ_MODDATE_EPOCH_TICKS);
		}
	}
}
