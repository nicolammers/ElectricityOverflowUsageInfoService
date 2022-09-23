using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectricityOverflowUsageInfoService.Extensions;
using ElectricityOverflowUsageInfoService.SmardApi.DTO;

namespace ElectricityOverflowUsageInfoService.Services {
    public class ElectricityUsageService {
        private readonly SmardApi.SmardApiReader _smardApiReader;

        public ElectricityUsageService(SmardApi.SmardApiReader smardApiReader) {
            _smardApiReader = smardApiReader;
        }

        /**
        * <summary>Gets the total electricity usage from the last 24h until now.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetTotalElectricityUsageForLastDayAsync() {
            List<DateTimeValueTuple> totalElectricityUsageForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityUsage)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => ((double) x[0]).toDateTime() > DateTime.Now.AddDays(-1) && ((double) x[0]).toDateTime() <= DateTime.Now)) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).toDateTime(),
                    Value = element[1]
                };

                totalElectricityUsageForLastDay.Add(dateTimeValueTuple);
            }

            return totalElectricityUsageForLastDay;
        }

        /**
        * <summary>Gets the total electricity usage from now until the SmardApi doesn't deliver new values.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetTotalElectricityUsageForFutureAsync() {
            List<DateTimeValueTuple> totalElectricityUsageForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityUsage)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).toDateTime() >= DateTime.Now)) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).toDateTime(),
                    Value = element[1]
                };

                totalElectricityUsageForFuture.Add(dateTimeValueTuple);
            }

            return totalElectricityUsageForFuture;
        }

        public async Task<List<DateTimeValueTuple>> GetTotalElectricityUsageAsync() {
            List<DateTimeValueTuple> totalElectricityGeneration = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(GetTotalElectricityUsageForLastDayAsync());
            asyncOperations.Add(GetTotalElectricityUsageForFutureAsync());

            await Task.WhenAll(asyncOperations);

            totalElectricityGeneration.AddRange(asyncOperations[0].Result);
            totalElectricityGeneration.AddRange(asyncOperations[1].Result);

            return totalElectricityGeneration;
        }
    }
}
