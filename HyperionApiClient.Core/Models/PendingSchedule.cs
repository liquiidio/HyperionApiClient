using Newtonsoft.Json;

namespace HyperionApiClient.Core.Models
{
    public class PendingSchedule
    {
        [JsonProperty("schedule_lib_num")]
        public int ScheduleLibNum { get; set; }

        [JsonProperty("schedule_hash")]
        public string ScheduleHash { get; set; }

        [JsonProperty("schedule")]
        public Schedule Schedule { get; set; }
    }
}