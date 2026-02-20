using System;

namespace EggDotNet.InternalExtensions
{
    internal static class DateUtilities
    {
		private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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

        public static DateTime FromEpoch(long timestamp)
        {
            return epoch.AddSeconds(timestamp);

		}
    }
}
