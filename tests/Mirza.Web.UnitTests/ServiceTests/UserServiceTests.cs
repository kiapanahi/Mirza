using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mirza.Web.Data;
using Mirza.Web.Models;
using Mirza.Web.Services.User;
using Xunit;

namespace Mirza.Web.UnitTests.ServiceTests
{
    public class UserServiceTests : IDisposable
    {
        private IUserService _userService;
        private MirzaDbContext _inMemoryDbContext;
        public UserServiceTests()
        {
            var contextOptions = new DbContextOptionsBuilder<MirzaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _inMemoryDbContext = new MirzaDbContext(contextOptions);

            _userService = new UserService(_inMemoryDbContext);
        }

        [Fact]
        public async Task Register_Null_User_ThrowsAsync()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.Register(null));
        }

        [Fact]
        public async Task Regsiter_Duplicate_Email_Throws()
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

            var user1RegisterResult = await _userService.Register(u1);

            await Assert.ThrowsAsync<DuplicateEmailException>(() => _userService.Register(u2));
        }

        [Fact]
        public async Task Register_Invalid_Model_ThrowsAsync()
        {
            var u = new MirzaUser
            {
                Email = Guid.NewGuid().ToString(),
                FirstName = new string('a', 60),
                LastName = new string('b', 100)
            };

            await Assert.ThrowsAsync<UserModelValidationException>(() => _userService.Register(u));
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

            var result = await _userService.Register(u);

            Assert.NotNull(result);

            Assert.True(result.Id > 0);
            Assert.Equal(u.Email, result.Email);
            Assert.Equal(u.FirstName, result.FirstName);
            Assert.Equal(u.LastName, result.LastName);
            Assert.True(result.IsActive);

            Assert.NotNull(result.Teams);
            Assert.Equal(0, result.Teams.Count);

            Assert.NotNull(result.AccessKeys);
            Assert.Equal(0, result.AccessKeys.Count);

            Assert.NotNull(result.WorkLog);
            Assert.Equal(0, result.WorkLog.Count);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeAll)
        {
            _inMemoryDbContext?.Dispose();
            _inMemoryDbContext = null;
            _userService = null;
        }
    }
}
