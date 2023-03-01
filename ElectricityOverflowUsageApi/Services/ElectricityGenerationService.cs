using ElectricityOverflowUsageInfoService.Extensions;
using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService.Services {
    public class ElectricityGenerationService {
        private readonly SmardApi.SmardApiReader _smardApiReader;

        public ElectricityGenerationService(SmardApi.SmardApiReader smardApiReader) {
            _smardApiReader = smardApiReader;
        }

        public async Task<List<DateTimeValueTuple>> GetTotalElectricityGenerationAsync() {
            List<DateTimeValueTuple> totalElecticityGeneration = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(GetTotalElectricityGenerationForLastDayAsync());
            asyncOperations.Add(GetTotalElectricityGenerationForFutureAsync());

            await Task.WhenAll(asyncOperations);

            totalElecticityGeneration.AddRange(asyncOperations[0].Result);
            totalElecticityGeneration.AddRange(asyncOperations[1].Result);

            return totalElecticityGeneration;
        }

        public async Task<List<DateTimeValueTuple>> GetWindPhotovolatikGenerationAsync() {
            List<DateTimeValueTuple> totalElecticityGeneration = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(GetWindPhotovolatikGenerationForLastDayAsync());
            asyncOperations.Add(GetWindPhotovoltaikGenerationForFutureAsync());

            await Task.WhenAll(asyncOperations);

            totalElecticityGeneration.AddRange(asyncOperations[0].Result);
            totalElecticityGeneration.AddRange(asyncOperations[1].Result);

            return totalElecticityGeneration;
        }

        /// <summary>
        /// Returns the Percent Value of Wind and Photovoltaik Generation of the total Generation
        /// in MWh for the last 24 hours in quarterhour differences until the aviable prgonoses
        /// </summary>
        public async Task<List<DateTimeValueTuple>> GetWindPhotovoltaikGenerationInPercentAsync() {
            List<DateTimeValueTuple> electricityOverflow = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(GetTotalElectricityGenerationAsync());
            asyncOperations.Add(GetWindPhotovolatikGenerationAsync());

            await Task.WhenAll(asyncOperations);

            List<DateTimeValueTuple> totalGeneration = asyncOperations[0].Result;
            List<DateTimeValueTuple> windPhotovoltaikGen = asyncOperations[1].Result;

            //For each element in generation and usage list subtract usage from generation values for each datetimes and create DateTimeValueTuple
            for (int i = 0; i < totalGeneration.Count; i++) {
                DateTime dateTime = totalGeneration[i].DateTime;
                double value = 0;

                if (windPhotovoltaikGen[i].Value != null && totalGeneration[i].Value != null) {
                    value = (double) ((windPhotovoltaikGen[i].Value * 100.00) / totalGeneration[i].Value );
                }

                electricityOverflow.Add(new DateTimeValueTuple() {
                    DateTime = dateTime,
                    Value = value
                });
            }

            return electricityOverflow;
        }

        /**
        * <summary>Gets the total electricity generation from now until the SmardApi doesn't deliver new values.</summary>
        */
        private async Task<List<DateTimeValueTuple>> GetTotalElectricityGenerationForFutureAsync() {
            List<DateTimeValueTuple> totalElecticityGenerationForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.TotalElectricityGenerationPrognosis)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.TotalElectricityGenerationPrognosis, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).ToDateTime() >= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                totalElecticityGenerationForFuture.Add(dateTimeValueTuple);
            }

            return totalElecticityGenerationForFuture;
        }


        /**
        * <summary>Gets the wind and photovoltaik electricity generation from now until the SmardApi doesn't deliver new values.</summary>
        */
        private async Task<List<DateTimeValueTuple>> GetWindPhotovoltaikGenerationForFutureAsync() {
            List<DateTimeValueTuple> windPhotovoltaikElecticityGenerationForFuture = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.PhotovoltaicWindGenerationPrognosis)).Timestamps.First();

            TimeSeries timeSeriesForTimestamp = await _smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.PhotovoltaicWindGenerationPrognosis, lastTimestamp);

            foreach (List<double?> element in timeSeriesForTimestamp.Series
                .Where(x => x[1] != null && ((double) x[0]).ToDateTime() >= DateTimeExtensions.DateTimeNowCorrection())) {

                DateTimeValueTuple dateTimeValueTuple = new DateTimeValueTuple() {
                    DateTime = ((double) element[0]).ToDateTime(),
                    Value = element[1]
                };

                windPhotovoltaikElecticityGenerationForFuture.Add(dateTimeValueTuple);
            }

            return windPhotovoltaikElecticityGenerationForFuture;
        }

        private async Task<List<DateTimeValueTuple>> GetTotalElectricityGenerationForLastDayAsync() {
            List<DateTimeValueTuple> totalElectricityGenerationForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.BiomassGeneration)).Timestamps.First();

            //Get all Timeseries for all kinds of electricitygeneration-types
            List<TimeSeries> timeSeriesElectricityGeneration = await GetTimeSeriesElectricityGenerationAsync(lastTimestamp);

            //Get all List of DateTimeValueTuple for each Electricity Generation Type for the last 24 hours
            List<List<DateTimeValueTuple>> dateTimeValueTupleListOfAllGenerationKinds = timeSeriesElectricityGeneration.
                Select(x => GetDateTimeValueTupleForLastDayAsync(x)).ToList();

            //For each element in DateTimeValueTupleList sum values for datetimes and create DateTimeValueTuple
            for (int i = 0; i < dateTimeValueTupleListOfAllGenerationKinds.First().Count; i++) {
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

        private async Task<List<DateTimeValueTuple>> GetWindPhotovolatikGenerationForLastDayAsync() {
            List<DateTimeValueTuple> totalWindPhotovoltaikGenerationForLastDay = new List<DateTimeValueTuple>();

            //Last Available Timestamp (Usually current week)
            double lastTimestamp = (await _smardApiReader.GetIndicesDescAsync(SmardApi.Filter.WindOffshoreGeneration)).Timestamps.First();

            //Get all Timeseries for all kinds of electricitygeneration-types
            List<TimeSeries> timeSeriesWindPhotovoltaikGeneration = await GetTimeSeriesWindPhotovoltaikGenerationAsync(lastTimestamp);

            //Get all List of DateTimeValueTuple for each Electricity Generation Type for the last 24 hours
            List<List<DateTimeValueTuple>> dateTimeValueTupleListOfWindPhotovoltaikGeneration = timeSeriesWindPhotovoltaikGeneration.
                Select(x => GetDateTimeValueTupleForLastDayAsync(x)).ToList();

            //For each element in DateTimeValueTupleList sum values for datetimes and create DateTimeValueTuple
            for (int i = 0; i < dateTimeValueTupleListOfWindPhotovoltaikGeneration.First().Count; i++) {
                double valueSum = 0;
                DateTime dateTime = dateTimeValueTupleListOfWindPhotovoltaikGeneration.First()[i].DateTime;

                //sum values for each element
                foreach (List<DateTimeValueTuple> dateTimeValueTuplesOfGenerationType in dateTimeValueTupleListOfWindPhotovoltaikGeneration) {
                    valueSum += dateTimeValueTuplesOfGenerationType[i].Value != null ? (double) dateTimeValueTuplesOfGenerationType[i].Value : 0;
                }

                totalWindPhotovoltaikGenerationForLastDay.Add(new DateTimeValueTuple() {
                    DateTime = dateTime,
                    Value = valueSum
                });
            }

            return totalWindPhotovoltaikGenerationForLastDay;
        }

        private async Task<List<TimeSeries>> GetTimeSeriesElectricityGenerationAsync(double timestamp) {
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

        private async Task<List<TimeSeries>> GetTimeSeriesWindPhotovoltaikGenerationAsync(double timestamp) {
            List<TimeSeries> timeSeries = new List<TimeSeries>();

            List<Task<TimeSeries>> asyncOperations = new List<Task<TimeSeries>>();

            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.WindOffshoreGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.WindOnshoreGeneration, timestamp));
            asyncOperations.Add(_smardApiReader.GetTimeSeriesAsync(SmardApi.Filter.PhotovoltaicGeneration, timestamp));

            await Task.WhenAll(asyncOperations);

            timeSeries.AddRange(asyncOperations.Select(x => x.Result).ToList());

            return timeSeries;
        }

        private List<DateTimeValueTuple> GetDateTimeValueTupleForLastDayAsync(TimeSeries timeSeries) {
            List<DateTimeValueTuple> dateTimeValueTuples = new List<DateTimeValueTuple>();

            foreach (List<double?> element in timeSeries.Series
                .Where(x => ((double) x[0]).ToDateTime() > DateTime.Now.AddDays(-1) &&
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
