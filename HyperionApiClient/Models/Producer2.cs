using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperionApiClient.Responses
{
    public class Producer2
    {
        [JsonProperty("producer_name")]
        public string ProducerName { get; set; }

        [JsonProperty("authority")]
        public List<object> Authority { get; set; }
    }
}