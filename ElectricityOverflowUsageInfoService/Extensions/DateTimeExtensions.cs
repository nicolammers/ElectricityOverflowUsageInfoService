using System;

namespace ElectricityOverflowUsageInfoService.Extensions {
    public static class DateTimeExtensions {
        private static readonly DateTime EPOCH_START_DATE = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime toDateTime(this double unixTimeStamp) {
            return EPOCH_START_DATE.AddMilliseconds(unixTimeStamp).ToLocalTime();
        }
    }
}
