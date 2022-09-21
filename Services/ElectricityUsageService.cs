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

        public async Task<List<DateTimeValueTuple>> GetTotalElecticityUsageForLastDayAsync() {
            try {
                List<DateTimeValueTuple> totalElecticityUsageForLastDay = new List<DateTimeValueTuple>();

                //Last Available Timestamp (Usually current week)
                double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityUsage)).Timestamps.First();

                TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

                foreach (List<double?> element in timeSeriesForTimestamp.Series.Where(x => ((double) x[0]).toDateTime() > DateTime.Now.AddDays(-1))) {
                    DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                        DateTime = ((double) element[0]).toDateTime(),
                        Value = element[1]
                    };

                    totalElecticityUsageForLastDay.Add(dateTimeValueTuple);
                }

                return totalElecticityUsageForLastDay;

            } catch (Exception ex) {
                //ToDo Exception handling
                throw ex;
            }
        }
    }
}
