using System.Linq;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService {
    internal class ProgramStart {
        static async Task Main(string[] args) {
           SmardApi.Indices indices = await SmardApi.SmardApiReader.GetIndicesAsync();

           await SmardApi.SmardApiReader.GetTimeSeriesAsync(indices.Timestamps.First());
        }
    }
}
