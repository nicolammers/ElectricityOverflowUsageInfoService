using ElectricityOverflowUsageInfoService.Extensions;
using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService {
    internal class ProgramStart {
        static async Task Main(string[] args) {
            List<Services.DateTimeValueTuple> lastDayGeneration = await (new Services.ElectricityGenerationService(new SmardApi.SmardApiReader()).GetTotalElectricityGenerationForLastDayAsync());
            foreach (Services.DateTimeValueTuple element in lastDayGeneration) {
                Console.WriteLine("                  " + element.DateTime);
                Console.WriteLine("                  " + element.Value);

                Console.WriteLine("");
                Console.WriteLine("");
            }


            //Indices indices = await (new SmardApi.SmardApiReader()).GetIndicesDescAsync(SmardApi.Filter.TotalElectricityGenerationPrognosis);

            ////foreach (double mainTs in indices.Timestamps) {
            //    double mainTs = indices.Timestamps.First();
            //    Console.WriteLine("Indices: " + mainTs.toDateTime().ToString());

            //    TimeSeries timeSeries = await (new SmardApi.SmardApiReader()).GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityGenerationPrognosis, mainTs);

            //    if (timeSeries != null && timeSeries.Series.Any()) {
            //        foreach (List<double?> element in timeSeries.Series) {
            //            Console.WriteLine("                  " + ((double) element[0]).toDateTime().ToString());
            //            Console.WriteLine("                  " + element[1].ToString());

            //            Console.WriteLine("");
            //            Console.WriteLine("");
            //        }
            //    } else if (timeSeries != null && !timeSeries.Series.Any()) {
            //        Console.WriteLine("No Subdata but Series found");
            //    } else {
            //        Console.WriteLine("No Series found for Timestamp");
            //    }

            ////    Console.WriteLine("");
            ////    Console.WriteLine("");
            ////    Console.WriteLine("");
            ////    Console.WriteLine("");
            ////    Console.WriteLine("");
            ////}
        }
    }
}
