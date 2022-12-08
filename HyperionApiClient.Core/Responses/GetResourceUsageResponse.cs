using HyperionApiClient.Core.Models;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Responses
{
    public class GetResourceUsageResponse
    {
        [JsonProperty("cpu")]
        public Cpu Cpu { get; set; }

        [JsonProperty("net")]
        public Net Net { get; set; }

        [JsonProperty("query_time_ms")]
        public double QueryTimeMs { get; set; }
    }
}