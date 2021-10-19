using System.Collections.Generic;
using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class GetKeyAccountsResponse
    {
        [JsonProperty("account_names", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> AccountNames { get; set; }
    }
}