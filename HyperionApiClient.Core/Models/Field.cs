using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class Field
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}