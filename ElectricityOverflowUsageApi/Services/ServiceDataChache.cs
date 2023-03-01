using ElectricityOverflowUsageInfoService.Services;
using ElectricityOverflowUsageInfoService.SmardApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ElectricityOverflowUsageApi.Services {
    public static class ServiceDataChache {

        public static List<DateTimeValueTuple> ElectricityOverflow { get; set; }
        public static List<DateTimeValueTuple> ElectricityGeneration { get; set; }
        public static List<DateTimeValueTuple> ElectricityUsage { get; set; }
        public static List<DateTimeValueTuple> ElectricityPrices { get; set; }
        public static List<DateTimeValueTuple> WindPhotovoltaikGenerationPercent { get; set; }

        private static readonly SmardApiReader _smardApiReader = new SmardApiReader();
        private static readonly ElectricityGenerationService _electricityGenerationService = new ElectricityGenerationService(_smardApiReader);
        private static readonly ElectricityUsageService _electricityUsageService = new ElectricityUsageService(_smardApiReader);
        private static readonly ElectricityOverflowService _electricityOverflowService = new ElectricityOverflowService(
            _electricityGenerationService,
            _electricityUsageService
            );
        private static readonly ElectricityPriceService _electricityPriceService = new ElectricityPriceService(_smardApiReader);


        private const int INTERVAL = 900000; //quarterhour

        //static constructor
        static ServiceDataChache() {
            RefreshData(); //initial setData

            var timer = new System.Timers.Timer();
            timer.Interval = INTERVAL;
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private static void TimerElapsed(Object source, ElapsedEventArgs e) {
            RefreshData();
        }

        private static void RefreshData() {
            Thread t = new Thread(() => {
                RefreshElecOverflowAsync();
                RefreshElecGenerationAsync();
                RefreshElecUsageAsync();
                RefreshElecPricesAsync();
            });

            t.Start();
        }

        private static async Task RefreshElecOverflowAsync() {
            ElectricityOverflow = await _electricityOverflowService.GetElectricityOverflowAsync();
        }

        private static async Task RefreshElecGenerationAsync() {
            ElectricityGeneration = await _electricityGenerationService.GetTotalElectricityGenerationAsync();
        }

        private static async Task RefreshElecUsageAsync() {
            ElectricityUsage = await _electricityUsageService.GetTotalElectricityUsageAsync();
        }

        private static async Task RefreshElecPricesAsync() {
            ElectricityPrices = await _electricityPriceService.GetElectricityPricesAsync();
        }

        private static async Task RefreshWindPhotovoltaikEnergyAsync() {
            WindPhotovoltaikGenerationPercent = await _electricityGenerationService.GetWindPhotovoltaikGenerationInPercentAsync();
        }
    }
}
