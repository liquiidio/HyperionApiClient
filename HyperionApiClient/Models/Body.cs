using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class Body 
    {
        /// <summary>public key</summary>
        [JsonProperty("public_key", Required = Required.Always)]
        public string PublicKey { get; set; }
    }
}