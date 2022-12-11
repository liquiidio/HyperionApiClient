﻿using System.Collections.Generic;
using HyperionApiClient.Core.Models;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Responses
{
    public class GetActivatedProtocolFeaturesResponse
    {
        [JsonProperty("activated_protocol_features")]
        public List<ActivatedProtocolFeature> ActivatedProtocolFeatures { get; set; }

        [JsonProperty("more")]
        public int More { get; set; }
    }
}