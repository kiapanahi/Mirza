using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mirza.Web.Models;
using Mirza.Web.Services.User;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests.UserServiceTests
{
    public class AccessKeyTests : UserServiceTestBase
    {
        [Fact]
        public async Task GetUserWithActiveAccessKey_ActiveAccessKey_Ok()
        {
            const string accessKeyValue = @"01234567890123456789012345678905";
            var accessKeyExpirationTime = DateTime.Parse("2020-10-01");
            const int userId = 5;


            var getUserWithAccessKeyResult = await UserService.GetUserWithActiveAccessKey(accessKeyValue);
            Assert.NotNull(getUserWithAccessKeyResult);
            Assert.Equal(2, getUserWithAccessKeyResult.AccessKeys.Count);
            Assert.Single(getUserWithAccessKeyResult.AccessKeys.Where(w => w.IsActive));
            Assert.Single(getUserWithAccessKeyResult.AccessKeys.Where(w => !w.IsActive));
            Assert.Equal(accessKeyValue, getUserWithAccessKeyResult.AccessKeys.First().Key);
            Assert.Equal(accessKeyExpirationTime, getUserWithAccessKeyResult.AccessKeys.First().Expiration);
            Assert.Equal(userId, getUserWithAccessKeyResult.Id);
        }

        [Fact]
        public async Task GetUserWithActiveAccessKey_InactiveAccessKey_Returns_Null()
        {
            var accessKeyValue = @"abcdefabcdefabcdefabcdefabcdef05";

            var getUserWithAccessKeyResult = await UserService.GetUserWithActiveAccessKey(accessKeyValue);
            Assert.Null(getUserWithAccessKeyResult);
        }

        [Fact]
        public async Task GetUserWithActiveAccessKey_EmptySet_Returns_Null()
        {
            var user = await UserService.GetUserWithActiveAccessKey(Guid.NewGuid().ToString());
            Assert.Null(user);
        }

        [Fact]
        public async Task AddAccessKey_NonExistingUser_Throws()
        {
            var result = await Assert.ThrowsAsync<AccessKeyException>(() => UserService.AddAccessKey(50));

            Assert.Equal("User id 50 does not exist", result.Message);
        }

        [Fact]
        public async Task AddAccessKey_InactiveUser_Throws()
        {
            var user = DbContext.UserSet.Find(10);
            user.IsActive = false;
            DbContext.SaveChanges();

            var result = await Assert.ThrowsAsync<AccessKeyException>(() => UserService.AddAccessKey(10));

            Assert.Equal("User id 10 is not active", result.Message);
        }

        [Fact]
        public async Task AddAccessKey_ValidData_Ok()
        {
            var result = await UserService.AddAccessKey(10);
            Assert.NotNull(result);
            Assert.Equal(DateTime.UtcNow.Date.AddYears(1), result.Expiration.Date);
            Assert.True(Guid.TryParse(result.Key, out _));
            Assert.Equal(10, result.OwnerId);
            Assert.Equal(AccessKeyState.Active, result.State);
        }

        [Fact]
        public async Task DeactivateAccessKey_NonExistingUser_Throws()
        {
            var result = await Assert.ThrowsAsync<AccessKeyException>(() => UserService.DeactivateAccessKey(50, "01234567890123456789012345678950"));

            Assert.Equal("User id 50 does not exist", result.Message);
        }

        [Fact]
        public async Task DeactivateAccessKey_InactiveUser_Throws()
        {
            var user = DbContext.UserSet.Find(10);
            user.IsActive = false;
            DbContext.SaveChanges();

            var result = await Assert.ThrowsAsync<AccessKeyException>(() => UserService.DeactivateAccessKey(10, "01234567890123456789012345678910"));

            Assert.Equal("User id 10 is not active", result.Message);
        }

        [Fact]
        public async Task DeactivateAccessKey_InvalidAccessKey_Throws()
        {
            var result = await Assert.ThrowsAsync<AccessKeyException>(() => UserService.DeactivateAccessKey(10, "01234567890123456789012345678940"));

            Assert.Equal("Invalid access key", result.Message);
        }

        [Fact]
        public async Task DeactivateAccessKey_InactiveAccessKey_Ok()
        {
            var user = DbContext.UserSet.Include(a => a.AccessKeys).Single(a => a.Id == 10);
            user.AccessKeys.First().State = AccessKeyState.Inative;
            DbContext.SaveChanges();

            var result = await UserService.DeactivateAccessKey(10, "01234567890123456789012345678910");
            Assert.NotNull(result);
            Assert.True(Guid.TryParse(result.Key, out _));
            Assert.Equal(10, result.OwnerId);
            Assert.Equal(AccessKeyState.Inative, result.State);
        }

        [Fact]
        public async Task DeactivateAccessKey_ValidData_Ok()
        {
            var result = await UserService.DeactivateAccessKey(10, "01234567890123456789012345678910");
            Assert.NotNull(result);
            Assert.True(Guid.TryParse(result.Key, out _));
            Assert.Equal(10, result.OwnerId);
            Assert.Equal(AccessKeyState.Inative, result.State);
        }
    }
}