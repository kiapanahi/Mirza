using System;

namespace Mirza.Web.Models
{
    public class WorkLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public MirzaUser User { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public DateTime LogDate { get; set; } = DateTime.Now;

    }
}
