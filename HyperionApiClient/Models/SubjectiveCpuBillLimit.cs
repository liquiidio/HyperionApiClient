using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public class SubjectiveCpuBillLimit
    {
        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("available")]
        public int Available { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}