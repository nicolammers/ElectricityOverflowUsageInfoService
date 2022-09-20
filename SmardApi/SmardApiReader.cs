using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService.SmardApi {
    public class SmardApiReader {

        public static string APIGATEWAY = "https://www.smard.de/app/chart_data";

        public async static Task<Indices> GetIndicesAsync() {
            try {
                using (HttpClient client = new HttpClient()) {
                    //ToDO: Timeout of CLient

                    HttpResponseMessage response = await client.GetAsync(new Uri(APIGATEWAY + "/410/DE/index_hour.json"));
                    string contentString = await response.Content.ReadAsStringAsync();
                    Indices indices = JsonConvert.DeserializeObject<Indices>(contentString);
                    return indices;
                }
            } catch (Exception ex) {
                //ToDo: exception handling
                return null;
            }
        }

        public async static Task<TimeSeries> GetTimeSeriesAsync(double timestamp) {
            try {
                using (HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.GetAsync(new Uri(APIGATEWAY + "/410/DE/410_DE_hour_" + timestamp.ToString() + ".json"));
                    string contentString = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(contentString);

                    TimeSeries timeSeries = JsonConvert.DeserializeObject<TimeSeries>(contentString);
                    return timeSeries;
                }
            } catch (Exception ex) {
                //ToDo: exception handling
                return null;
            }
        }
    }
}
