using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
