using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mirza.Web.Pages
{
    [Authorize]
    public class AboutMirzaModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}