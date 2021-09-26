using System.Collections.Generic;
using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class GetControlledAccountsResponse 
    {
        [JsonProperty("controlled_accounts", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> ControlledAccounts { get; set; }
    }
}