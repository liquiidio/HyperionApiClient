using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class Tokens 
    {
        [JsonProperty("symbol", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Symbol { get; set; }
    
        [JsonProperty("precision", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int Precision { get; set; }
    
        [JsonProperty("amount", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Amount { get; set; }
    
        [JsonProperty("contract", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Contract { get; set; }
    }
}