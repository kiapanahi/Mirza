using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Mirza.Web.Services.User;

namespace Mirza.Web.Areas.Identity.Pages.Account.Manage
{
    public class AccessKeyModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly UserManager<MirzaUser> _userManager;
        private readonly ILogger<AccessKeyModel> _logger;

        public AccessKeyModel(IUserService userService, UserManager<MirzaUser> userManager, ILogger<AccessKeyModel> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEnumerable<AccessKeyListItem> AccessKeyList { get; set; }

        public class InputModel
        {

        }



        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var keys = await _userService.GetAllAccessKeys(user.Id, false).ConfigureAwait(false);
            AccessKeyList = keys.Select(s => new AccessKeyListItem(s));
            return Page();
        }

        public async Task<IActionResult> OnPostDeactivateAccessKeyAsync(int accessKeyId)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                var deactivationResult = await _userService.DeactivateAccessKey(user.Id, accessKeyId).ConfigureAwait(false);
            }
            catch (AccessKeyException e)
            {
                ModelState.TryAddModelError("AccessKeyException", e.Message);
            }

            return RedirectToPage();
        }
    }
}
