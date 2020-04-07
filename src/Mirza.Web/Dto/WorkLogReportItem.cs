using System;
using Mirza.Web.Models;

namespace Mirza.Web.Dto
{
    public class WorkLogReportItem
    {
        public DateTime WorkLogDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
    }

    public static class WorkLogExtensions
    {
        public static WorkLogReportItem ToWorkLogReportItem(this WorkLog workLog)
        {
            if (workLog== null)
            {
                throw new ArgumentNullException(nameof(workLog));
            }
            return new WorkLogReportItem
            {
                Description = workLog.Description,
                Details = workLog.Details,
                EndTime = workLog.EndTime,
                WorkLogDate = workLog.EntryDate,
                StartTime = workLog.StartTime
            };
        }
    }
}