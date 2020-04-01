using System.Threading.Tasks;
using Mirza.Web.Models;

namespace Mirza.Web.Services.User
{
    public interface IUserService
    {
        Task<MirzaUser> Register(MirzaUser user);
    }
}
