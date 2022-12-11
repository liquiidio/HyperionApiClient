using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class Specification
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}