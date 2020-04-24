using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mirza.Common;

namespace Mirza.Cli.Models
{
    internal class WorkLogReport
    {
        [JsonPropertyName("workLogDate")]
        public DateTime ReportDate { get; set; }

        [JsonIgnore]
        public string ReportDatePersian => Utils.GetPersianDate(ReportDate);

        public IEnumerable<WorkLogReportItem> WorkLogItems { get; set; } = new List<WorkLogReportItem>();

        [JsonPropertyName("totalDurationString")]
        public string TotalDuration { get; set; }
    }
}