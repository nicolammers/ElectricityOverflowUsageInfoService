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
        public async Task<List<DateTimeValueTuple>> GetTotalElecticityUsageForLastDayAsync() {
            List<DateTimeValueTuple> totalElecticityUsageForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityUsage)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => ((double) x[0]).ToDateTime() > DateTimeExtensions.DateTimeNowCorrection().AddDays(-1) && ((double) x[0]).ToDateTime() <= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                totalElecticityUsageForLastDay.Add(dateTimeValueTuple);
            }

            return totalElecticityUsageForLastDay;
        }

        /**
        * <summary>Gets the total electricity usage from now until the SmardApi doesn't deliver new values.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetTotalElecticityUsageForFutureAsync() {
            List<DateTimeValueTuple> totalElecticityUsageForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityUsage)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).ToDateTime() >= DateTimeExtensions.DateTimeNowCorrection())) {
                
                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                totalElecticityUsageForFuture.Add(dateTimeValueTuple);
            }

            return totalElecticityUsageForFuture;
        }
    }
}
