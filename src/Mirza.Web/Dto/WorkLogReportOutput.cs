using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mirza.Web.Dto
{
    public class WorkLogReportOutput
    {
        public DateTime WorkLogDate { get; set; }

        public IEnumerable<WorkLogReportItem> WorkLogItems { get; set; } = new List<WorkLogReportItem>();

        public TimeSpan TotalDuration
            => WorkLogItems.Aggregate(TimeSpan.Zero,
                (current, item) => current.Add(item.EndTime - item.StartTime));

        public string TotalDurationString
            => TotalDuration.ToString("hh\\:mm\\:ss", CultureInfo.CurrentCulture);
    }
}