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

        private static void SeedUsers(MirzaDbContext context)
        {
            var users = Enumerable.Range(1, 30).Select(idx => new MirzaUser
            {
                AccessKeys = new List<AccessKey>
                {
                    new AccessKey($"012345678901234567890123456789{idx.ToString().PadLeft(2, '0')}")
                        {OwnerId = idx, State = AccessKeyState.Active, Expiration = DateTime.Parse("2020-10-01")},
                    new AccessKey("abcdefabcdefabcdefabcdefabcdefab")
                        {OwnerId = idx, State = AccessKeyState.Inative, Expiration = DateTime.Parse("2020-10-01")}
                },
                Email = $"user{idx}@example.com",
                TeamId = idx % 5 + 1,
                IsActive = true,
                FirstName = $"firstname {idx}",
                LastName = $"lastname {idx}"
            });
            context.UserSet.AddRange(users);
            context.SaveChanges();
        }

        private static void SeedTeams(MirzaDbContext context)
        {
            var teams = Enumerable.Range(1, 5).Select(idx => new Team {Name = $"team-{idx}"});
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