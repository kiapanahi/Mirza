﻿using System;
using System.Globalization;
using System.Linq;
using Mirza.Web.Models;

namespace Mirza.Web.Dto
{
    public class WorkLogReportItem
    {
        public DateTime WorkLogDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int Id { get; set; }
        public WorkLogTagItem[] Tags { get; set; }
    }

    public class WorkLogTagItem
    {
        public int Id { get; }
        public string Value { get; }

        public WorkLogTagItem(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }

    public static class WorkLogExtensions
    {
        public static WorkLogReportItem ToWorkLogReportItem(this WorkLog workLog)
        {
            if (workLog == null)
            {
                throw new ArgumentNullException(nameof(workLog));
            }

            return new WorkLogReportItem
            {
                Id = workLog.Id,
                Description = workLog.Description,
                Details = workLog.Details,
                EndTime = workLog.EndTime.ToString("hh\\:mm", CultureInfo.CurrentCulture),
                WorkLogDate = workLog.EntryDate,
                StartTime = workLog.StartTime.ToString("hh\\:mm", CultureInfo.CurrentCulture),
                Tags = workLog.Tags.Select(s => new WorkLogTagItem(s.Id, s.Value)).ToArray()
            };
        }
    }
}