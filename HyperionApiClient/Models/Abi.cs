using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HyperionApiClient.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class Action
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("ricardian_contract")]
        public string RicardianContract { get; set; }
    }

    public class Abi
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("types")]
        public List<object> Types { get; set; }

        [JsonProperty("structs")]
        public List<Struct> Structs { get; set; }

        [JsonProperty("actions")]
        public List<Action> Actions { get; set; }

        [JsonProperty("tables")]
        public List<Table> Tables { get; set; }

        [JsonProperty("ricardian_clauses")]
        public List<RicardianClaus> RicardianClauses { get; set; }

        [JsonProperty("error_messages")]
        public List<object> ErrorMessages { get; set; }

        [JsonProperty("abi_extensions")]
        public List<object> AbiExtensions { get; set; }

        [JsonProperty("variants")]
        public List<object> Variants { get; set; }
    }
}
