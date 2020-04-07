using System;
using System.Linq;
using System.Threading.Tasks;
using Mirza.Web.Models;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests.UserServiceTests
{
    public class ReportTests : UserServiceTestBase
    {
        private async Task<int> AddWorkLog(int userId, TimeSpan start, TimeSpan end, string description = "-",
            string detail = "-", DateTime logDate = default)
        {
            if (logDate == default)
            {
                logDate = DateTime.Today.Date;
            }

            var addResult = await DbContext.WorkLogSet.AddAsync(new WorkLog
            {
                Description = description,
                Details = detail,
                UserId = userId,
                StartTime = start,
                EntryDate = logDate,
                EndTime = end
            });

            return addResult.Entity.Id;
        }

        [Fact]
        public async Task GetWorkLogReport_ValidData_Ok()
        {
            var ids = await Task.WhenAll(
                AddWorkLog(1, TimeSpan.Parse("8:00"), TimeSpan.Parse("9:30"), "desc 1", "detail 1",
                    DateTime.Today.Date),
                AddWorkLog(1, TimeSpan.Parse("9:30"), TimeSpan.Parse("11:00"), "desc 2", "detail 2",
                    DateTime.Today.Date),
                AddWorkLog(1, TimeSpan.Parse("11:30"), TimeSpan.Parse("15:00"), "desc 3", "detail 3",
                    DateTime.Today.Date),
                AddWorkLog(1, TimeSpan.Parse("8:00"), TimeSpan.Parse("10:00"), "desc 1", "detail 1",
                    DateTime.Today.Date.AddDays(-1)),
                AddWorkLog(1, TimeSpan.Parse("11:00"), TimeSpan.Parse("19:00"), "desc 2", "detail 2",
                    DateTime.Today.Date.AddDays(-1)),
                AddWorkLog(2, TimeSpan.Parse("8:00"), TimeSpan.Parse("9:30"), "desc 1", "detail 1",
                    DateTime.Today.Date),
                AddWorkLog(2, TimeSpan.Parse("11:30"), TimeSpan.Parse("15:00"), "desc 3", "detail 3",
                    DateTime.Today.Date),
                AddWorkLog(2, TimeSpan.Parse("8:00"), TimeSpan.Parse("10:00"), "desc 1", "detail 1",
                    DateTime.Today.Date.AddDays(-1))
            );

            _ = await DbContext.SaveChangesAsync();

            var report = await UserService.GetWorkLogReport(1, DateTime.Today.Date);
            Assert.NotNull(report);
            Assert.Equal(DateTime.Today.Date, report.WorkLogDate);
            Assert.Equal(3, report.WorkLogItems.Count());
            Assert.Equal(TimeSpan.Parse("06:30:00"), report.TotalDuration);
        }
    }
}