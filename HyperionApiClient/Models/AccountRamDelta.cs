using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class AccountRamDelta 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("delta", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }
}