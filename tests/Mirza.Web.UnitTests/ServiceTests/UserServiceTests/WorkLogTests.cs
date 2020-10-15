using System;
using System.Linq;
using System.Threading.Tasks;
using Mirza.Web.Models;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests.UserServiceTests
{
    public class WorkLogTests : UserServiceTestBase
    {

        [Fact]
        public async Task AddWorkLog_ValidData_Returns_WorkLog()
        {
            var workLog = new WorkLog
            {
                Description = "sample description",
                Details = "sample details",
                EntryDate = DateTime.Today.Date,
                StartTime = TimeSpan.Parse("9:15:00"),
                EndTime = TimeSpan.Parse("15:0:0")
            };
            var result = await UserService.AddWorkLog(1, workLog);
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(1, result.UserId);
            Assert.Equal(DateTime.Today.Date, result.EntryDate.Date);
            Assert.Equal(TimeSpan.Parse("5:45:00"), result.EndTime - result.StartTime);

        }

        [Fact]
        public async Task AddWorkLog_OverlappingRecords_Throws()
        {
            var workLog1 = new WorkLog
            {
                Description = "sample description",
                Details = "sample details",
                EntryDate = DateTime.Today.Date,
                StartTime = TimeSpan.Parse("9:15:00"),
                EndTime = TimeSpan.Parse("15:0:0")
            };
            var result = await UserService.AddWorkLog(1, workLog1);
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(1, result.UserId);
            Assert.Equal(DateTime.Today.Date, result.EntryDate.Date);
            Assert.Equal(TimeSpan.Parse("5:45:00"), result.EndTime - result.StartTime);

            var workLog2 = new WorkLog
            {
                Description = "sample description 2",
                Details = "sample details 2",
                EntryDate = DateTime.Today.Date,
                StartTime = workLog1.StartTime.Add(TimeSpan.FromMinutes(5)),
                EndTime = TimeSpan.Parse("16:0:0")
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => UserService.AddWorkLog(1, workLog2));
        }

        [Fact]
        public async Task DeleteWorkLog_ValidData_Returns_WorkLog()
        {
            await SetupWorkLogItems();

            var deletedWorkLog = await UserService.DeleteWorkLog(userId: 1, workLogId: 1);

            Assert.NotNull(deletedWorkLog);
            Assert.Equal(1, deletedWorkLog.Id);
            Assert.Equal("worklog 1", deletedWorkLog.Description);
            Assert.Equal("worklog detail 1", deletedWorkLog.Details);
            Assert.Equal(DateTime.Today.Date, deletedWorkLog.EntryDate);
            Assert.Equal(TimeSpan.Parse("8:30:0"), deletedWorkLog.StartTime);
            Assert.Equal(TimeSpan.Parse("9:0:0"), deletedWorkLog.EndTime);

            var userWorkLogs = await UserService.GetWorkLogReport(1, DateTime.Today.Date);
            Assert.Single(userWorkLogs.WorkLogItems);

            var remainingWorkLog = userWorkLogs.WorkLogItems.First();
            Assert.Equal(2, remainingWorkLog.Id);
            Assert.Equal("worklog 2", remainingWorkLog.Description);
            Assert.Equal("worklog detail 2", remainingWorkLog.Details);
            Assert.Equal(DateTime.Today.Date, remainingWorkLog.WorkLogDate);
            Assert.Equal("09:45", remainingWorkLog.StartTime);
            Assert.Equal("15:15", remainingWorkLog.EndTime);
        }


        [Fact]
        public async Task DeleteWorkLog_InvalidUser_Throws()
        {
            await SetupWorkLogItems();

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => UserService.DeleteWorkLog(100, 1));

            Assert.Equal("Invalid userId (Parameter 'userId')", exception.Message);
        }

        [Fact]
        public async Task DeleteWorkLog_InvalidWorkLog_Throws()
        {
            await SetupWorkLogItems();

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => UserService.DeleteWorkLog(1, 5));

            Assert.Equal("Work log not found. Id: 5 (Parameter 'workLogId')", exception.Message);
        }

        private async Task SetupWorkLogItems()
        {
            var workLog1 = new WorkLog
            {
                Description = "worklog 1",
                Details = "worklog detail 1",
                EntryDate = DateTime.Today.Date,
                StartTime = TimeSpan.Parse("08:30:00"),
                EndTime = TimeSpan.Parse("9:0:0")
            };
            _ = await UserService.AddWorkLog(1, workLog1);

            var workLog2 = new WorkLog
            {
                Description = "worklog 2",
                Details = "worklog detail 2",
                EntryDate = DateTime.Today.Date,
                StartTime = TimeSpan.Parse("09:45:00"),
                EndTime = TimeSpan.Parse("15:15:0")
            };
            _ = await UserService.AddWorkLog(1, workLog2);
        }
    }
}
