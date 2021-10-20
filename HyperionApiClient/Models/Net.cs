﻿using Newtonsoft.Json;

namespace HyperionApiClient.Models
{
    public class Net
    {
        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("percentiles")]
        public Percentiles Percentiles { get; set; }
    }
}