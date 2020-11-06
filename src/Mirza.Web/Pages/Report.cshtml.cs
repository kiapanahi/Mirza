using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Mirza.Web.Services.Report;

namespace Mirza.Web.Pages
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class ReportModel : PageModel
    {
        private readonly IReportService _reportService;
        private readonly UserManager<MirzaUser> _userManager;

        public ReportModel(IReportService reportService, UserManager<MirzaUser> userManager)
        {
            _reportService = reportService;
            _userManager = userManager;
        }

        public IEnumerable<WorkLogReportOutput> Report { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            var mirzaUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (mirzaUser is null)
            {
                return NotFound(new
                {
                    Error = "User not found"
                });
            }

            Report = await _reportService.GetReportAsync(mirzaUser).ConfigureAwait(false);

            return Page();
        }
    }
}