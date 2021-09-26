using System;
using System.Collections.Generic;
using System.Text;
using EosRio.HyperionApi;
using Newtonsoft.Json;

namespace HyperionApiClient.Models
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
