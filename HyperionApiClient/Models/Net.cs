using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class Net
    {
        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("percentiles")]
        public Percentiles Percentiles { get; set; }
    }
}