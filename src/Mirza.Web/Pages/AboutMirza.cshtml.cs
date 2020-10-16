using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mirza.Web.Pages
{
    [Authorize]
    public class AboutMirzaModel : PageModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public void OnGet()
        {

        }
    }
}