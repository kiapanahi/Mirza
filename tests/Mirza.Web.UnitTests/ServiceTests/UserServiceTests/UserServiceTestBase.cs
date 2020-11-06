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
        protected MirzaDbContext DbContext { get; private set; }

        public UserServiceTestBase()
        {
            var contextOptions = new DbContextOptionsBuilder<MirzaDbContext>()
                                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                 .Options;

            DbContext = new MirzaDbContext(contextOptions);

            SeedData(DbContext);

            UserService = new UserService(DbContext, NullLogger<UserService>.Instance);
        }

        protected virtual void SeedData(MirzaDbContext context)
        {
            SeedTenants(context);
            SeedTeams(context);
            SeedUsers(context);
        }

        protected virtual void SeedTenants(MirzaDbContext context)
        {
            var tenantData = Enumerable.Range(1, 5).Select(idx => new MirzaTenant
            {
                Name = $"tenant-{idx}",
                Description = $"The great tenant {idx}",
                Teams = Enumerable.Range(1, 5).Select(i => CreateDummyTeam(i)).ToList(),
                Members = Enumerable.Range(1, 30).Select(i => CreateDummyUser(i)).ToList()
            });
            context.TenantSet.AddRange(tenantData);
            context.SaveChanges();
        }
        protected virtual void SeedTeams(MirzaDbContext context) { }
        protected virtual void SeedUsers(MirzaDbContext context) { }

        private MirzaTeam CreateDummyTeam(int idx) => new MirzaTeam { Name = $"team-{idx}" };
        private MirzaUser CreateDummyUser(int idx) => new MirzaUser
        {
            AccessKeys = new List<AccessKey>
                {
                    new AccessKey($"012345678901234567890123456789{idx.ToString().PadLeft(2, '0')}")
                        {OwnerId = idx, State = AccessKeyState.Active, Expiration = DateTime.Parse("2020-10-01")},
                    new AccessKey($"abcdefabcdefabcdefabcdefabcdef{idx.ToString().PadLeft(2, '0')}")
                        {OwnerId = idx, State = AccessKeyState.Inative, Expiration = DateTime.Parse("2020-10-01")}
                },
            Email = $"user{idx}@example.com",
            IsActive = true,
            FirstName = $"firstname {idx}",
            LastName = $"lastname {idx}"
        };


        protected IUserService UserService { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeAll)
        {
            DbContext?.Dispose();
            DbContext = null;
            UserService = null;
        }
    }
}