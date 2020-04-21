using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mirza.Web.Data;
using Mirza.Web.Dto;

namespace Mirza.Web.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly MirzaDbContext _dbContext;

        public ReportService(MirzaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WorkLogReportOutput>> GetReportAsync()
        {
            var workLogItems = await _dbContext.WorkLogSet.Include(w => w.User)
                .OrderByDescending(w => w.LogDate)
                .ThenByDescending(w => w.UserId)
                .ToListAsync().ConfigureAwait(false);

            var result = workLogItems.GroupBy(g => new { Email = g.User.Email, WorkLogDate = g.EntryDate.Date })
                .Select(g => new WorkLogReportOutput
                {
                    User = g.Key.Email,
                    WorkLogDate = g.Key.WorkLogDate,
                    WorkLogItems = g.Select(i => i.ToWorkLogReportItem())
                })
                .ToList();

            return result;
        }
    }
}
