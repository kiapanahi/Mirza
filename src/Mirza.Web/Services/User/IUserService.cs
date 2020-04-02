using System.Threading.Tasks;
using Mirza.Web.Models;

namespace Mirza.Web.Services.User
{
    public interface IUserService
    {
        Task<MirzaUser> Register(MirzaUser user);
        Task<MirzaUser> GetUserWithActiveAccessKey(string accessKey);
        Task<MirzaUser> GetUser(int id);
        Task DeleteUser(int id);
    }
}
