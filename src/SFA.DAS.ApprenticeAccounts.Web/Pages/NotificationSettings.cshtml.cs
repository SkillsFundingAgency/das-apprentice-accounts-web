using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

                var apprenticePreferences = new List<ApprenticePreference>();

                apprenticePreferences = preferencesDto.Result.Select(p => new ApprenticePreference
                {
                    PreferenceId = p.PreferenceId,
                    PreferenceMeaning = p.PreferenceMeaning,
                    Status = ((apprenticePreferencesDto.Result.ApprenticePreferences.ToList()
                        .FirstOrDefault(ap => ap.PreferenceId == p.PreferenceId)).Status) ?? (bool)null
                });

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
            if (!ModelState.IsValid)
            {
                if (ModelState.ErrorCount > 0) ModelState.AddModelError("MultipleErrorSummary", "Select Yes or No");

                return Page();
            }

            BackLink = _urlHelper.Generate(NavigationSection.Home, "Home");
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