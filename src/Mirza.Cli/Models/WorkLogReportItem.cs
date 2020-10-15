using System;
using System.Text.Json.Serialization;

namespace Mirza.Cli.Models
{
    internal class WorkLogReportItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("workLogDate")]
        public DateTime WorkLogDate { get; set; }

        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }
    }
}