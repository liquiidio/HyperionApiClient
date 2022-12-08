using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class RequiredAuth
    {
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        [JsonProperty("keys")]
        public List<string> Keys { get; set; }

        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }

        [JsonProperty("waits")]
        public List<string> Waits { get; set; }
    }
}