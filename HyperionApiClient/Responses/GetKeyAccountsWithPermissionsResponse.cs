using System.Collections.Generic;
using HyperionApiClient.Core.Models;
using Newtonsoft.Json;

namespace HyperionApiClient.Core.Responses
{
    public class GetKeyAccountsWithPermissionsResponse : GetKeyAccountsResponse
    {
        [JsonProperty("permissions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Permission> Permissions { get; set; }
    }
}