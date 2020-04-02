using System;
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
    }
}
