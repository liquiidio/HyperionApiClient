using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class Schedule
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("producers")]
        public List<object> Producers { get; set; }
    }
}