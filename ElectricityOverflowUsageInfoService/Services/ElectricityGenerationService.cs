using ElectricityOverflowUsageInfoService.Extensions;
using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService.Services {
    public class ElectricityGenerationService {

        private readonly SmardApi.SmardApiReader _smardApiReader;

        public ElectricityGenerationService(SmardApi.SmardApiReader smardApiReader) {
            _smardApiReader = smardApiReader;
        }

        /**
        * <summary>Gets the total electricity generation from now until the SmardApi doesn't deliver new values.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetTotalElecticityGenerationForFutureAsync() {
            List<DateTimeValueTuple> totalElecticityGenerationForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityGenerationPrognosis)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityUsage, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).toDateTime() >= DateTime.Now)) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).toDateTime(),
                    Value = element[1]
                };

                totalElecticityGenerationForFuture.Add(dateTimeValueTuple);
            }

            return totalElecticityGenerationForFuture;
        }
    }
}
