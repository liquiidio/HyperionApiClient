using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class ExtendedLinks : Links
    {
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }

        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }

        [JsonProperty("irreversible", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Irreversible { get; set; }
    }
}