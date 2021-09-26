using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class Stats 
    {
        [JsonProperty("by_producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object ByProducer { get; set; }
    }
}