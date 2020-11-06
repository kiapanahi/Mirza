using System;
using System.Threading.Tasks;
using Mirza.Web.Models;
using Mirza.Web.Services.User;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests.UserServiceTests
{
    public class RegisterUserTests : UserServiceTestBase
    {
        [Fact]
        public async Task Register_Duplicate_Email_Throws()
        {
            var u1 = new MirzaUser
            {
                FirstName = "first_name_1",
                LastName = "last_name_1",
                Email = "email@example.com"
            };

            var u2 = new MirzaUser
            {
                FirstName = "first_name_2",
                LastName = "last_name_2",
                Email = "email@example.com"
            };

            _ = await UserService.Register(u1);

            await Assert.ThrowsAsync<DuplicateEmailException>(() => UserService.Register(u2));
        }

        [Fact]
        public async Task Register_Invalid_Model_Throws()
        {
            var u = new MirzaUser
            {
                Email = Guid.NewGuid().ToString(),
                FirstName = new string('a', 60),
                LastName = new string('b', 100)
            };

            await Assert.ThrowsAsync<UserModelValidationException>(() => UserService.Register(u));
        }

        [Fact]
        public async Task Register_Null_User_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => UserService.Register(null));
        }

        [Fact]
        public async Task Register_Valid_Model_Ok()
        {
            var u = new MirzaUser
            {
                Email = "email@example.com",
                FirstName = "sample_firstname",
                LastName = "sample_lastname"
            };

            var result = await UserService.Register(u);

            Assert.NotNull(result);

            Assert.True(result.Id > 0);
            Assert.Equal(u.Email, result.Email);
            Assert.Equal(u.FirstName, result.FirstName);
            Assert.Equal(u.LastName, result.LastName);
            Assert.True(result.IsActive);

            Assert.Null(result.Teams);

            Assert.NotNull(result.AccessKeys);
            Assert.Empty(result.AccessKeys);

            Assert.NotNull(result.WorkLog);
            Assert.Empty(result.WorkLog);
        }
    }
}