﻿using System.Collections.Generic;
using HyperionApiClient.Core.Models;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Responses
{
    public class GetTokensResponse
    {
        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("tokens")]
        public List<Token> Tokens { get; set; }

        [JsonProperty("query_time_ms")]
        public double QueryTimeMs { get; set; }
    }
}