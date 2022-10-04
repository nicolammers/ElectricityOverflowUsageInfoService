using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService.Services {
    public class ElectricityOverflowService {
        private readonly ElectricityGenerationService _electricityGenerationService;
        private readonly ElectricityUsageService _electricityUsageService;

        public ElectricityOverflowService(
            ElectricityGenerationService generationService,
            ElectricityUsageService usageService) {
            _electricityGenerationService = generationService;
            _electricityUsageService = usageService;
        }

        /// <summary>
        /// Returns OverflowElectricity in MWh for the last 24 hours in quarterhour differences until the aviable prgonoses
        /// </summary>
        public async Task<List<DateTimeValueTuple>> GetElectricityOverflowAsync() {
            List<DateTimeValueTuple> electricityOverflow = new List<DateTimeValueTuple>();

            List<Task<List<DateTimeValueTuple>>> asyncOperations = new List<Task<List<DateTimeValueTuple>>>();
            asyncOperations.Add(_electricityGenerationService.GetTotalElectricityGenerationAsync());
            asyncOperations.Add(_electricityUsageService.GetTotalElectricityUsageAsync());

            await Task.WhenAll(asyncOperations);

            List<DateTimeValueTuple> generation = asyncOperations[0].Result;
            List<DateTimeValueTuple> usage = asyncOperations[1].Result;

            //For each element in generation and usage list subtract usage from generation values for each datetimes and create DateTimeValueTuple
            for (int i = 0; i < generation.Count; i++) {
                DateTime dateTime = generation[i].DateTime;
                double value = generation[i].Value != null ? (double) generation[i].Value : 0;
                value = usage[i].Value != null ? (value - (double) usage[i].Value) : value;

                electricityOverflow.Add(new DateTimeValueTuple() {
                    DateTime = dateTime,
                    Value = value
                });
            }

            return electricityOverflow;
        }
    }
}
