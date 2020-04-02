using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Mirza.Web.Data;
using Mirza.Web.Services.User;

namespace Mirza.Web.UnitTests.ServiceTests
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

            UserService = new UserService(_inMemoryDbContext, new NullLogger<UserService>());
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
