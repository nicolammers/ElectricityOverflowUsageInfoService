using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService {
    internal class ProgramStart {
        static async Task Main(string[] args) {
            await SmardApi.SmardApiReader.Search();
        }
    }
}
