using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class AccountRamDelta 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("delta", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }
}