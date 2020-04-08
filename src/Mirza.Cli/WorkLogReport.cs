using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Mirza.Cli
{
    public class WorkLogReport
    {
        private readonly PersianCalendar _pc = new PersianCalendar();

        /// <summary>
        ///     Report Date
        /// </summary>
        [JsonPropertyName("workLogDate")]
        public DateTime ReportDate { get; set; }

        /// <summary>
        ///     Report Date in Persian
        /// </summary>
        [JsonIgnore]
        public string ReportDatePersian => Utils.GetPersianDate(ReportDate);

        /// <summary>
        ///     Work Log entries
        /// </summary>
        public IEnumerable<WorkLogReportItem> WorkLogItems { get; set; } = new List<WorkLogReportItem>();

        [JsonPropertyName("totalDurationString")]
        public string TotalDuration { get; set; }
    }

    public class WorkLogReportItem
    {
        /// <summary>
        /// Entry Id
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Entry date
        /// </summary>
        [JsonPropertyName("workLogDate")]
        public DateTime WorkLogDate { get; set; }

        /// <summary>
        ///     Entry start time
        /// </summary>
        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }

        /// <summary>
        ///     Entry end time
        /// </summary>
        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }

        /// <summary>
        ///     Entry description or "-"
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Entry description details or "-"
        /// </summary>
        [JsonPropertyName("details")]
        public string Details { get; set; }
    }
}