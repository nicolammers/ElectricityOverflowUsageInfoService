using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService
{
    class ProgramStart
    {
        static async Task Main(string[] args)
        {
            await SmardApiReader.Search();
        }
    }
}
