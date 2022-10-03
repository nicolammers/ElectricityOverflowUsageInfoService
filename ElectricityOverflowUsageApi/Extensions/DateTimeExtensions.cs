using System;

namespace ElectricityOverflowUsageInfoService.Extensions {
    public static class DateTimeExtensions {
        private static readonly DateTime EPOCH_START_DATE = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(this double unixTimeStamp) {
            return EPOCH_START_DATE.AddMilliseconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// Shifts the current point in time by 2 hours into the past,
        /// since the actual values from the realized data do not reach exactly up to the current point in time.
        /// It is therefore necessary to access the forecasts at an early stage.
        /// </summary>
        public static DateTime DateTimeNowCorrection() {
            return DateTime.Now.AddHours(-2);
        }
    }
}
