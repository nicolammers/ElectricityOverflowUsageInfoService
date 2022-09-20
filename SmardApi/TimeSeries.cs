using System;
using System.Collections.Generic;
using System.Text;

namespace ElectricityOverflowUsageInfoService.SmardApi {
    public class TimeSeries {
        public MetaData Meta_Data { get; set; }

        public List<List<double?>> Series { get; set; }
    }
}
