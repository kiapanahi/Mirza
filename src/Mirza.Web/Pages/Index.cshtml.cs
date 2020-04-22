using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mirza.Web.Dto;
using Mirza.Web.Models;
using Mirza.Web.Services.User;

namespace Mirza.Web.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly UserManager<MirzaUser> _userManager;

        public IndexModel(IUserService userService, UserManager<MirzaUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public WorkLogReportOutput TodayWorkLog { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "تاریخ")]
            [DataType(DataType.Date)]
            [Required(AllowEmptyStrings = false, ErrorMessage = "کاری که تاریخ نداره رو من تو کدوم صفحه‌ی تقویم بنویسم؟")]
            public DateTime Date { get; set; } = DateTime.Now;

            [Display(Name = "ساعت شروع")]
            [DataType(DataType.Time)]
            [Required(AllowEmptyStrings = false, ErrorMessage = "کاری که ساعت شروع نداشته باشه که نمی‌شه. نه؟")]
            public string Start { get; set; }

            [Display(Name = "ساعت پایان")]
            [DataType(DataType.Time)]
            [Required(AllowEmptyStrings = false, ErrorMessage = "کاری که تموم نشده رو چطوری ثبت می‌کنی؟")]
            public string End { get; set; }

            [Display(Name = "توضیح")]
            [DataType(DataType.MultilineText)]
            [StringLength(500, ErrorMessage = "چه خبرته؟ توضیحات رو به ۵۰۰ کاراکتر محدود کن!")]
            public string Description { get; set; }

            [Display(Name = "جزئیات")]
            [DataType(DataType.MultilineText)]
            [StringLength(500, ErrorMessage = "چه خبرته؟ جزئیات رو به ۵۰۰ کاراکتر محدود کن!")]
            public string Details { get; set; }

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input = new InputModel();

            TodayWorkLog = await _userService.GetWorkLogReport(user.Id, DateTime.Now.Date).ConfigureAwait(false);

            string latestWorkItemEndTime;

            if (TodayWorkLog == null || !TodayWorkLog.WorkLogItems.Any())
            {
                var now = DateTime.Now;
                var minutes = now.Minute;
                minutes -= (minutes % 5);

                latestWorkItemEndTime = $"{now:HH}:{minutes:D2}";
            }
            else
            {
                latestWorkItemEndTime = TodayWorkLog.WorkLogItems.OrderByDescending(w => w.EndTime).First().EndTime;
            }

            Input.Start = latestWorkItemEndTime;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                _ = TimeSpan.TryParse(Input.Start, out var startTime);
                _ = TimeSpan.TryParse(Input.End, out var endTime);

                _ = await _userService.AddWorkLog(user.Id, new WorkLog
                {
                    User = user,
                    UserId = user.Id,
                    Description = Input.Description ?? "-",
                    Details = Input.Details ?? "-",
                    EntryDate = Input.Date,
                    StartTime = startTime,
                    EndTime = endTime
                }).ConfigureAwait(false);
            }
            catch (ArgumentNullException)
            {
                ErrorMessage = "خالیه! چی رو بنویسم؟";
            }
            catch (WorkLogModelValidationException)
            {
                ErrorMessage = "من این زبون رو نفهمیدم. لطفن ورودی‌هات رو دوباره چک کن.";
            }
            catch (ArgumentException)
            {
                ErrorMessage = "بله؟ شما؟";
            }
            catch (InvalidOperationException)
            {
                ErrorMessage = "مگه میشه تو یه بازه‌ی زمانی دو تا کار کرده باشی؟ هان؟‌ میشه؟";
            }
            catch (Exception)
            {
                ErrorMessage = "یه اتفاقی افتاده عجیب! نمی‌دونم چی‌کار کنم!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteWorkLogAsync(int workLogId)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            try
            {
                var deletedWorkLog = await _userService.DeleteWorkLog(user.Id, workLogId).ConfigureAwait(false);
            }
            catch (ArgumentException e) when (e.Message.Contains("Invalid userId", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorMessage = "بله؟ شما؟";
            }
            catch (ArgumentException e) when (e.Message.Contains("Work log not found", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorMessage = "کاری رو که می‌خوای پاک کنم پیدا نکردم :)";
            }
            catch (Exception)
            {
                ErrorMessage = "یه اتفاقی افتاده عجیب! نمی‌دونم چی‌کار کنم!";
            }
            return RedirectToPage();
        }
    }
}