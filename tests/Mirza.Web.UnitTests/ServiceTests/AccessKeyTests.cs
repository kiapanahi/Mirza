using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirza.Web.Models;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests
{
    public class AccessKeyTests : UserServiceTestBase
    {
        [Fact]
        public async Task GetUserWithActiveAccessKey_EmptySet_Returns_Null()
        {
            var user = await UserService.GetUserWithActiveAccessKey(Guid.NewGuid().ToString());
            Assert.Null(user);
        }

        [Fact]
        public async Task GetUserWithActiveAccessKey_ActiveAccessKey_Ok()
        {
            var accessKeyValue = @"12345678-1234-5678-9012-123456789abc";
            var accessKeyExpirationTime = DateTime.UtcNow.AddMonths(6);
            var user = new MirzaUser
            {
                FirstName = "sample_user",
                LastName = "sample_lastname",
                Email = "sample@example.com",
                IsActive = true,
                AccessKeys = new List<AccessKey>
                {
                    new AccessKey
                    {
                        Key = accessKeyValue,
                        State = AccessKeyState.Active,
                        Expriation = accessKeyExpirationTime
                    }
                }
            };
            var saveResult = await UserService.Register(user);
            Assert.NotNull(saveResult);
            Assert.True(saveResult.Id > 0);
            Assert.Equal(1, saveResult.AccessKeys.Count);
            Assert.Equal(accessKeyValue, saveResult.AccessKeys.First().Key);
            Assert.Equal(accessKeyExpirationTime, saveResult.AccessKeys.First().Expriation);

            var getUserWithAccessKeyResult = await UserService.GetUserWithActiveAccessKey(accessKeyValue);
            Assert.NotNull(getUserWithAccessKeyResult);
            Assert.Equal(1, getUserWithAccessKeyResult.AccessKeys.Count);
            Assert.Equal(accessKeyValue, getUserWithAccessKeyResult.AccessKeys.First().Key);
            Assert.Equal(accessKeyExpirationTime, getUserWithAccessKeyResult.AccessKeys.First().Expriation);
            Assert.Equal(saveResult.Id, getUserWithAccessKeyResult.Id);
        }

        [Fact]
        public async Task GetUserWithActiveAccessKey_ActiveAccessKey_Returns_Null()
        {
            var accessKeyValue = @"12345678-1234-5678-9012-123456789abc";
            var accessKeyExpirationTime = DateTime.UtcNow.AddMonths(6);
            var user = new MirzaUser
            {
                FirstName = "sample_user",
                LastName = "sample_lastname",
                Email = "sample@example.com",
                IsActive = true,
                AccessKeys = new List<AccessKey>
                {
                    new AccessKey
                    {
                        Key = accessKeyValue,
                        State = AccessKeyState.Inative,
                        Expriation = accessKeyExpirationTime
                    }
                }
            };
            var saveResult = await UserService.Register(user);
            Assert.NotNull(saveResult);
            Assert.True(saveResult.Id > 0);
            Assert.Equal(1, saveResult.AccessKeys.Count);
            Assert.Equal(accessKeyValue, saveResult.AccessKeys.First().Key);
            Assert.Equal(accessKeyExpirationTime, saveResult.AccessKeys.First().Expriation);

            var getUserWithAccessKeyResult = await UserService.GetUserWithActiveAccessKey(accessKeyValue);
            Assert.Null(getUserWithAccessKeyResult);
        }
    }
}
