using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperionApiClient.Responses
{
    public class ActiveSchedule
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("producers")]
        public List<Producer2> Producers { get; set; }
    }
}