using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectricityOverflowUsageInfoService.Extensions;
using ElectricityOverflowUsageInfoService.Services;
using ElectricityOverflowUsageInfoService.SmardApi.DTO;

namespace ElectricityOverflowUsageApi.Services {
    public class ElectricityPriceService {
        private readonly ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader _smardApiReader;

        public ElectricityPriceService(ElectricityOverflowUsageInfoService.SmardApi.SmardApiReader smardApiReader) {
            _smardApiReader = smardApiReader;
        }

        /**
        * <summary>Gets the electricity prices from the last 24h until now.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetElectricityPricesForLastDayAsync() {
            List<DateTimeValueTuple> electricityPricesForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(ElectricityOverflowUsageInfoService.SmardApi.Filter.MarketPriceGermany)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(ElectricityOverflowUsageInfoService.SmardApi.Filter.MarketPriceGermany, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => ((double) x[0]).ToDateTime() > DateTimeExtensions.DateTimeNowCorrection().AddDays(-1) && ((double) x[0]).ToDateTime() <= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                electricityPricesForLastDay.Add(dateTimeValueTuple);
            }

            return electricityPricesForLastDay;
        }

        /**
        * <summary>Gets the electricity prices from now until the SmardApi doesn't deliver new values.</summary>
        */
        public async Task<List<DateTimeValueTuple>> GetElectricityPricesForFutureAsync() {
            List<DateTimeValueTuple> electricityPricesForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(ElectricityOverflowUsageInfoService.SmardApi.Filter.MarketPriceGermany)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(ElectricityOverflowUsageInfoService.SmardApi.Filter.MarketPriceGermany, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).ToDateTime() >= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                electricityPricesForFuture.Add(dateTimeValueTuple);
            }

            return electricityPricesForFuture;
        }

        public async Task<List<DateTimeValueTuple>> GetElectricityPricesAsync() {
            List<DateTimeValueTuple> electricityPrices = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(GetElectricityPricesForLastDayAsync());
            asyncOperations.Add(GetElectricityPricesForFutureAsync());

            await Task.WhenAll(asyncOperations);

            electricityPrices.AddRange(asyncOperations[0].Result);
            electricityPrices.AddRange(asyncOperations[1].Result);

            return electricityPrices;
        }
    }
}