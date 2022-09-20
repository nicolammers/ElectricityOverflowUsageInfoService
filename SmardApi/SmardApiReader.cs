using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService {
    public class SmardApiReader {

        public static string APIGATEWAY = "https://www.smard.de/app/chart_data/410/DE/index_hour.json";

        public async static Task Search() {
            try {
                using (HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.GetAsync(new Uri(APIGATEWAY));
                    string contentString = await response.Content.ReadAsStringAsync();
                    Indices indices = JsonConvert.DeserializeObject<Indices>(contentString);
                }
            } catch (Exception exception) {
                Console.WriteLine(exception);
            }
        }
    }
}
