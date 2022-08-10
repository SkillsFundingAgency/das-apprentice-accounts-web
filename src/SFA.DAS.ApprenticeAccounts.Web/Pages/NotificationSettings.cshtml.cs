using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    [HideNavigationBar]
    public class NotificationSettingsModel : PageModel
    {
        private readonly ApprenticeApi _apprenticeApi;
        private readonly NavigationUrlHelper _urlHelper;

        public NotificationSettingsModel(ApprenticeApi apprenticeApi, NavigationUrlHelper urlHelper)
        {
            _apprenticeApi = apprenticeApi;
            _urlHelper = urlHelper;
        }

        [BindProperty] public List<ApprenticePreference> ApprenticePreferences { get; set; }
        [BindProperty] public string BackLink { get; set; }
        [BindProperty] public bool SubmittedPreferences { get; set; }


        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            SubmittedPreferences = false;

            try
            {
                var preferencesDto = _apprenticeApi.GetAllPreferences();
                var apprenticePreferencesDto =
                    _apprenticeApi.GetAllApprenticePreferencesForApprentice(user.ApprenticeId);
                await Task.WhenAll(preferencesDto, apprenticePreferencesDto);

               var apprenticePreferences = preferencesDto.Result.Select(p => new ApprenticePreference
                {
                    PreferenceId = p.PreferenceId,
                    PreferenceMeaning = p.PreferenceMeaning,
                    PreferenceHint = p.PreferenceHint,
                    Status = apprenticePreferencesDto.Result.ApprenticePreferences
                        .FirstOrDefault(ap => ap.PreferenceId == p.PreferenceId)?.Status
                }).ToList();

                ApprenticePreferences = apprenticePreferences;
                BackLink = _urlHelper.Generate(NavigationSection.Home, "Home");

                return Page();
            }
            catch
            {
                return Redirect("Error");
            }
        }

        public async Task<IActionResult> OnPostAsync(
            [FromServices] AuthenticatedUser user)
        {
            BackLink = _urlHelper.Generate(NavigationSection.Home, "Home");

            if (!ModelState.IsValid)
            {
                if (ModelState.ErrorCount > 1)
                {
                    ModelState.AddModelError("MultipleErrorSummary", "Select Yes or No");
                    ViewData["MultipleErrorAnchor"] = "#preferenceRadioGroup0";
                }

                return Page();
            }

            SubmittedPreferences = true;

            try
            {
                var apprenticePreferencesCommand = new ApprenticePreferencesCommand
                {
                    ApprenticePreferences = ApprenticePreferences.Select(apprenticePreferences =>
                        new ApprenticePreferenceCommand
                        {
                            ApprenticeId = user.ApprenticeId,
                            PreferenceId = apprenticePreferences.PreferenceId,
                            Status = apprenticePreferences.Status
                        })
                };

                 
                await _apprenticeApi.UpdateAllApprenticePreferences(apprenticePreferencesCommand);

                return Page();
            }
            catch
            {
                return Redirect("Error");
            }
        }
    }
}