using System;
using System.Threading.Tasks;
using Mirza.Web.Dto;
using Mirza.Web.Models;

namespace Mirza.Web.Services.User
{
    public interface IUserService
    {
        Task<MirzaUser> Register(MirzaUser user);
        Task<MirzaUser> GetUserWithActiveAccessKey(string accessKey);
        Task<MirzaUser> GetUser(int id);
        Task DeleteUser(int id);
        Task<WorkLog> AddWorkLog(int userId, WorkLog workLog);
        Task<AccessKey> AddAccessKey(int userId);
        Task<AccessKey> DeactivateAccessKey(int userId, string accessKey);
        Task<WorkLogReportOutput> GetWorkLogReport(int userId, DateTime logDate);
    }
}
