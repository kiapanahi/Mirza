using System;
using System.Collections.Generic;
using System.Linq;
using Mirza.Web.Models;

namespace Mirza.Web.Dto
{
    public class WorkLogReportOutput
    {
        public DateTime WorkLogDate { get; set; }

        public IEnumerable<WorkLog> WorkLogItems { get; set; } = new List<WorkLog>();

        public TimeSpan TotalDuration
            => WorkLogItems.Aggregate(TimeSpan.Zero,
                (current, item) => current.Add(item.EndTime - item.StartTime));
    }
}