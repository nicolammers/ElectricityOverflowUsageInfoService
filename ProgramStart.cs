using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService {
    internal class ProgramStart {
        static async Task Main(string[] args) {
            Indices indices = await (new SmardApi.SmardApiReader()).GetIndicesAsync();
            indices.Timestamps.Reverse();

            //foreach (double mainTs in indices.Timestamps) {
                Console.WriteLine("Indices: " + UnixTimeStampToDateTime(indices.Timestamps.First()).ToString());

                TimeSeries timeSeries = await (new SmardApi.SmardApiReader()).GetTimeSeriesAsync(indices.Timestamps.First());

                if(timeSeries != null && timeSeries.Series.Any()) {
                    foreach (List<double?> element in timeSeries.Series) {
                        Console.WriteLine("                  " + UnixTimeStampToDateTime((double) element[0]));
                        Console.WriteLine("                  " + element[1].ToString());

                        Console.WriteLine("");
                        Console.WriteLine("");
                    }
                } else if (timeSeries != null && !timeSeries.Series.Any()) {
                    Console.WriteLine("No Subdata but Series found");
                } else {
                    Console.WriteLine("No Series found for Timestamp");
                }

            //    Console.WriteLine("");
            //    Console.WriteLine("");
            //    Console.WriteLine("");
            //    Console.WriteLine("");
            //    Console.WriteLine("");
            //}
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
