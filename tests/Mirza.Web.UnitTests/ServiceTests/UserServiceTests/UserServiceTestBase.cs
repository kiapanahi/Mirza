using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Mirza.Web.Data;
using Mirza.Web.Models;
using Mirza.Web.Services.User;

namespace Mirza.Web.UnitTests.ServiceTests.UserServiceTests
{
    public class UserServiceTestBase : IDisposable
    {
        private MirzaDbContext _inMemoryDbContext;
        public UserServiceTestBase()
        {
            var contextOptions = new DbContextOptionsBuilder<MirzaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _inMemoryDbContext = new MirzaDbContext(contextOptions);

            SeedData(_inMemoryDbContext);

            UserService = new UserService(_inMemoryDbContext, new NullLogger<UserService>());
        }

        private void SeedData(MirzaDbContext context)
        {
            SeedTeams(context);
            SeedUsers(context);
        }

        private void SeedUsers(MirzaDbContext context)
        {
            var users = Enumerable.Range(1, 5).Select(idx => new MirzaUser
            {
                AccessKeys = new List<AccessKey> { new AccessKey("01234567890123456789012345678901") },
                Email = $"user{idx}@example.com",
                IsActive = true,
                FirstName = $"firstname {idx}",
                LastName = $"lastname {idx}"
            });
            context.UserSet.AddRange(users);
            context.SaveChanges();
        }

        private void SeedTeams(MirzaDbContext context)
        {
            var teams = Enumerable.Range(1, 5).Select(idx => new Team { Name = $"team-{idx}" });
            context.TeamSet.AddRange(teams);
            context.SaveChanges();
        }

        protected IUserService UserService { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeAll)
        {
            _inMemoryDbContext?.Dispose();
            _inMemoryDbContext = null;
            UserService = null;
        }
    }
}
