using ElectricityOverflowUsageInfoService.SmardApi.DTO;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ElectricityOverflowUsageInfoService.SmardApi {
    public class SmardApiReader {
        public async Task<Indices> GetIndicesDescAsync(Filter filter) {
            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(GetIndicesUri(filter));
                string contentString = await response.Content.ReadAsStringAsync();
                Indices indices = JsonConvert.DeserializeObject<Indices>(contentString);

                indices.Timestamps.Reverse();
                return indices;
            }
        }

        private Uri GetIndicesUri(Filter filter) {
            return new Uri(
                Constants.APIGATEWAY +
                Constants.PATH_SEPERATOR +
                (int) filter +
                Constants.PATH_SEPERATOR +
                Constants.LOCATION +
                Constants.PATH_SEPERATOR +
                Constants.INDEX +
                Constants.BOTTOM_LINE +
                Constants.HOUR +
                Constants.JSON_END
                );
        }

        public async Task<TimeSeries> GetTimeSeriesAsync(Filter filter, double timestamp) {
            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(GetTimeSeriesUri(filter, timestamp));
                string contentString = await response.Content.ReadAsStringAsync();
                TimeSeries timeSeries = JsonConvert.DeserializeObject<TimeSeries>(contentString);
                return timeSeries;
            }

        }

        private Uri GetTimeSeriesUri(Filter filter, double timestamp) {
            return new Uri(
                Constants.APIGATEWAY +
                Constants.PATH_SEPERATOR +
                (int) filter +
                Constants.PATH_SEPERATOR +
                Constants.LOCATION +
                Constants.PATH_SEPERATOR +
                (int) filter +
                Constants.BOTTOM_LINE +
                Constants.LOCATION +
                Constants.BOTTOM_LINE +
                Constants.QUARTERHOUR +
                Constants.BOTTOM_LINE +
                timestamp.ToString() +
                Constants.JSON_END
                );
        }
    }
}
