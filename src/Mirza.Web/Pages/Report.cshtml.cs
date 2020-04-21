using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirza.Web.Dto;
using Mirza.Web.Services.Report;

namespace Mirza.Web.Pages
{
    public class ReportModel : PageModel
    {
        private readonly IReportService _reportService;

        public ReportModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IEnumerable<WorkLogReportOutput> Report { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Report = await _reportService.GetReportAsync().ConfigureAwait(false);

            return Page();
        }
    }
}