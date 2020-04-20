using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mirza.Web.Data;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Mirza.Web.Validators;

namespace Mirza.Web.Services.User
{
    public class UserService : IUserService
    {
        private readonly MirzaDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly UserValidator _userValidator;
        private readonly WorkLogValidator _workLogValidator;

        public UserService(MirzaDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userValidator = new UserValidator();
            _workLogValidator = new WorkLogValidator();
        }

        #region AccessKey Manipulation

        public async Task<AccessKey> AddAccessKey(int userId)
        {
            var user = await _dbContext.UserSet.Include(u => u.AccessKeys)
                                       .SingleOrDefaultAsync(s => s.Id == userId)
                                       .ConfigureAwait(false);

            if (user == null)
            {
                throw new AccessKeyException($"User id {userId} does not exist");
            }

            if (!user.IsActive)
            {
                throw new AccessKeyException($"User id {userId} is not active");
            }

            try
            {
                var accessKey = new AccessKey(Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture))
                {
                    OwnerId = userId
                };
                user.AccessKeys.Add(accessKey);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return accessKey;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured while adding new access key for user {userId}", e);
                throw;
            }
        }

        public async Task<AccessKey> DeactivateAccessKey(int userId, int accessKeyId)
        {
            var accessKey = await _dbContext.AccessKeySet
                .Where(w => w.OwnerId == userId)
                .SingleOrDefaultAsync(s => s.Id == accessKeyId)
                .ConfigureAwait(false);

            if (accessKey == null)
            {
                throw new AccessKeyException("Invalid access key Id");
            }

            return await DeactivateAccessKey(userId, accessKey.Key).ConfigureAwait(false);
        }

        public async Task<AccessKey> DeactivateAccessKey(int userId, string accessKey)
        {
            var user = await _dbContext.UserSet.Include(u => u.AccessKeys)
                                       .SingleOrDefaultAsync(s => s.Id == userId)
                                       .ConfigureAwait(false);

            if (user == null)
            {
                throw new AccessKeyException($"User id {userId} does not exist");
            }

            if (!user.IsActive)
            {
                throw new AccessKeyException($"User id {userId} is not active");
            }

            if (!user.AccessKeys.Any(a => a.Key == accessKey))
            {
                throw new AccessKeyException("Invalid access key");
            }

            var found = user.AccessKeys.Single(a =>
                string.Equals(accessKey, a.Key, StringComparison.OrdinalIgnoreCase));
            if (!found.IsActive)
            {
                // Do noting here, the access key is already not active due to
                // expiration or state.
                return found;
            }

            found.State = AccessKeyState.Inative;
            try
            {
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return found;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception occured while deactivating accesskey: {found.Id}", e);
                throw;
            }
        }

        public async Task<MirzaUser> GetUserWithActiveAccessKey(string accessKey)
        {
            try
            {
                var foundUser = await _dbContext.UserSet
                                                .Include(a => a.AccessKeys)
                                                .Include(t => t.Team)
                                                .SingleOrDefaultAsync(
                                                    user => user.IsActive &&
                                                            user.AccessKeys.Any(
                                                                ak => ak.State == AccessKeyState.Active &&
                                                                      ak.Expiration >= DateTime.UtcNow &&
                                                                      accessKey == ak.Key)
                                                )
                                                .ConfigureAwait(false);
                return foundUser;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occured while querying for user with access key", e);
                return null;
            }
        }

        public async Task<IEnumerable<AccessKey>> GetAllAccessKeys(int userId, bool activeOnly = false)
        {
            var user = await _dbContext.UserSet.Include(u => u.AccessKeys)
                .SingleOrDefaultAsync(a => a.Id == userId)
                .ConfigureAwait(false);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var keys = user.AccessKeys.ToList();
            if (activeOnly)
            {
                keys = keys.Where(w => w.IsActive).ToList();
            }

            return keys;
        }


        #endregion

        #region WorkLog Manipulation

        public async Task<WorkLog> AddWorkLog(int userId, WorkLog workLog)
        {
            if (workLog == null)
            {
                throw new ArgumentNullException(nameof(workLog));
            }

            var validationResult = _workLogValidator.Validate(workLog);
            if (!validationResult.IsValid)
            {
                throw new WorkLogModelValidationException(validationResult
                                                          .Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var user = await _dbContext.UserSet
                                       .Include(u => u.WorkLog)
                                       .SingleOrDefaultAsync(s => s.IsActive && s.Id == userId)
                                       .ConfigureAwait(false);

            if (user == null)
            {
                throw new ArgumentException("Invalid userId", nameof(userId));
            }

            var overlappingLogExists = user.WorkLog.Any(w =>
                w.EntryDate.Date == workLog.EntryDate.Date &&
                (workLog.StartTime >= w.StartTime && workLog.StartTime < w.EndTime
                 || workLog.EndTime > w.StartTime && workLog.StartTime < w.StartTime));

            if (overlappingLogExists)
            {
                throw new InvalidOperationException("Can not log overlapping work periods for a given date");
            }

            try
            {
                var addResult = await _dbContext.WorkLogSet.AddAsync(new WorkLog
                {
                    Description = workLog.Description ?? "-",
                    Details = workLog.Details ?? "-",
                    EntryDate = workLog.EntryDate.Date,
                    StartTime = workLog.StartTime,
                    EndTime = workLog.EndTime,
                    UserId = user.Id
                });
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return addResult.Entity;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occured while saving worklog", e);
                throw;
            }
        }

        public async Task<WorkLog> DeleteWorkLog(int userId, int workLogId)
        {
            var user = await _dbContext.UserSet
                                       .Include(u => u.WorkLog)
                                       .SingleOrDefaultAsync(s => s.IsActive && s.Id == userId)
                                       .ConfigureAwait(false);

            if (user == null)
            {
                throw new ArgumentException("Invalid userId", nameof(userId));
            }

            var workLogToRemove = user.WorkLog.SingleOrDefault(w => w.Id == workLogId);
            if (workLogToRemove == null)
            {
                throw new ArgumentException($"Work log not found. Id: {workLogId}", nameof(workLogId));
            }
            if (user.WorkLog.Remove(workLogToRemove))
            {
                try
                {
                    _ = await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                    return workLogToRemove;
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception occured while deleting worklog", e);
                    throw;
                }

            }
            else
            {
                throw new InvalidOperationException($"Error occured while trying to remove worklog {workLogId} " +
                    $"from user: {userId} work log collection");
            }

        }

        public async Task<WorkLogReportOutput> GetWorkLogReport(int userId, DateTime logDate)
        {
            try
            {
                var logItems = await _dbContext.WorkLogSet
                                               .Where(w => w.UserId == userId && w.EntryDate.Date == logDate.Date)
                                               .Select(s => s.ToWorkLogReportItem())
                                               .ToListAsync()
                                               .ConfigureAwait(false);
                return new WorkLogReportOutput
                {
                    WorkLogDate = logDate,
                    WorkLogItems = logItems
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occured while compiling work log report." +
                                 $"user: {userId} date: {logDate}", e);
                throw;
            }
        }

        #endregion

        #region User Manipulaiton

        public async Task DeleteUser(int id)
        {
            try
            {
                var user = await _dbContext.UserSet.FindAsync(id);
                user.IsActive = false;
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occured while marking user as deleted", e);
                throw;
            }
        }

        public async Task<MirzaUser> GetUser(int id)
        {
            return await _dbContext.UserSet.FindAsync(id);
        }

        public async Task<MirzaUser> Register(MirzaUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var validationResult = _userValidator.Validate(user);
            if (!validationResult.IsValid)
            {
                throw new UserModelValidationException(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var duplicateEmail = await _dbContext.UserSet
                                                 .AnyAsync(u => u.IsActive && u.Email == user.Email)
                                                 .ConfigureAwait(false);

            if (duplicateEmail)
            {
                throw new DuplicateEmailException(user.Email);
            }

            try
            {
                await _dbContext.UserSet.AddAsync(user);
                await _dbContext.SaveChangesAsync().ConfigureAwait(true);
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception occured while saving user entity", e);
                throw;
            }
        }

        #endregion
    }
}