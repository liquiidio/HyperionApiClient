using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class AuthSequence 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Sequence { get; set; }
    }
}