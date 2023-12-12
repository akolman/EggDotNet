using System;

namespace EggDotNet.Extensions
{
	internal static class Utilities
	{
		private const long MODDATE_EPOCH_TICKS = 504911232000000000;

		public static DateTime FromEggTime(long timeVal)
		{
			return new DateTime(timeVal + MODDATE_EPOCH_TICKS);
		}
	}
}
