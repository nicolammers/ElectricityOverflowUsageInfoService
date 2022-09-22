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

        public async Task<List<DateTimeValueTuple>> GetTotalElecticityGenerationForLastDayAsync() {
            List<DateTimeValueTuple> totalElectricityGenerationForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.BiomassGeneration)).Timestamps.First();

            //Get all Timeseries for all kinds of electricitygeneration-types
            List<TimeSeries> timeSeriesElectricityGeneration = await GetTimeSeriesElecticityGenerationAsync(lastTimestamp);

            //Get all List of DateTimeValueTuple for each Electricity Generation Type for the last 24 hours
            List<List<DateTimeValueTuple>> dateTimeValueTupleListOfAllGenerationKinds = timeSeriesElectricityGeneration.
                Select(x => GetDateTimeValueTupleForLastDayAsync(x)).ToList();

            //For each element in DateTimeValueTupleList sum values for datetimes and create DateTimeValueTuple
            for (int i = 0; i < dateTimeValueTupleListOfAllGenerationKinds.First().Count - 1; i++) {
                double valueSum = 0;
                DateTime dateTime = dateTimeValueTupleListOfAllGenerationKinds.First()[i].DateTime;

                //sum values for each element
                foreach (List<DateTimeValueTuple> dateTimeValueTuplesOfGenerationType in dateTimeValueTupleListOfAllGenerationKinds) {
                    valueSum += dateTimeValueTuplesOfGenerationType[i].Value != null ? (double) dateTimeValueTuplesOfGenerationType[i].Value : 0;
                }

                totalElectricityGenerationForLastDay.Add(new DateTimeValueTuple() {
                    DateTime = dateTime,
                    Value = valueSum
                });
            }

            return totalElectricityGenerationForLastDay;
        }

        private async Task<List<TimeSeries>> GetTimeSeriesElecticityGenerationAsync(double timestamp) {
            List<TimeSeries> timeSeries = new List<TimeSeries>();

            List<Task<TimeSeries>> asyncOperations = new List<Task<TimeSeries>>();

            //Get all Timeseries for all kinds of electricitygeneration-types
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.BiomassGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.HydropowerGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.WindOffshoreGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.WindOnshoreGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.PhotovoltaicGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.OtherRenewableGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.NuclearEnergyGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.BrownCoalGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.HardCoalGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.NaturalGasGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.PumpStorageGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.OtherConventionalGeneration, timestamp));

            await Task.WhenAll(asyncOperations);

            timeSeries.AddRange(asyncOperations.Select(x => x.Result).ToList());

            return timeSeries;
        }

        private List<DateTimeValueTuple> GetDateTimeValueTupleForLastDayAsync(TimeSeries timeSeries) {
            List<DateTimeValueTuple> dateTimeValueTuples = new List<DateTimeValueTuple>();

            foreach (List<double?> element in timeSeries.Series
                .Where(x => ((double) x[0]).ToDateTime() > DateTimeExtensions.DateTimeNowCorrection().AddDays(-1) &&
                ((double) x[0]).ToDateTime() <= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                dateTimeValueTuples.Add(dateTimeValueTuple);
            }

            return dateTimeValueTuples;
        }
    }
}
