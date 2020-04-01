using System;
using System.Threading.Tasks;
using Mirza.Web.Data;
using Mirza.Web.Models;

namespace Mirza.Web.Services.User
{
    public class UserService : IUserService
    {
        private readonly MirzaDbContext _dbContext;

        public UserService(MirzaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<MirzaUser> Register(MirzaUser user)
        {
            throw new NotImplementedException();
        }
    }
}
