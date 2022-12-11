using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Responses
{
    public class GetControlledAccountsResponse 
    {
        [JsonProperty("controlled_accounts", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> ControlledAccounts { get; set; }
    }
}