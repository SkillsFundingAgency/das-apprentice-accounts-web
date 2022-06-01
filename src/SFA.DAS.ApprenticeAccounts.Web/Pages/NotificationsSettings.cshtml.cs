using System.Collections.Generic;
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
    public class NotificationsSettingsModel : PageModel
    {
        private readonly ApprenticeApi _apprenticeApi;
        private readonly NavigationUrlHelper _urlHelper;

        public NotificationsSettingsModel(ApprenticeApi apprenticeApi, NavigationUrlHelper urlHelper)
        {
            _apprenticeApi = apprenticeApi;
            _urlHelper = urlHelper;
        }

        [BindProperty] public List<ApprenticePreferenceCombination> ApprenticePreferences { get; set; }
        [BindProperty] public string BackLink { get; set; }
        [BindProperty] public bool SubmittedPreferences { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            SubmittedPreferences = false;

            try
            {
                var preferences = _apprenticeApi.GetPreferences().Result;
                var apprenticePreferences = _apprenticeApi.GetApprenticePreferences(user.ApprenticeId).Result;

                var combination = new List<ApprenticePreferenceCombination>();

                foreach (Preference preference in preferences)
                {
                    var apprenticePreference =
                        apprenticePreferences.ApprenticePreferences.Find(a =>
                            a.PreferenceId == preference.PreferenceId);
                    if (apprenticePreference == null)
                    {
                        var tempObject = new ApprenticePreferenceCombination()
                        {
                            PreferenceId = preference.PreferenceId,
                            PreferenceMeaning = preference.PreferenceMeaning,
                            Status = null
                        };
                        combination.Add(tempObject);
                    }
                    else
                    {
                        var tempObject = new ApprenticePreferenceCombination()
                        {
                            PreferenceId = preference.PreferenceId,
                            PreferenceMeaning = preference.PreferenceMeaning,
                            Status = apprenticePreference.Status
                        };
                        combination.Add(tempObject);
                    }

                    ApprenticePreferences = combination;
                }

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
                if (ModelState.ErrorCount > 0)
                {
                    ModelState.AddModelError("MultipleErrorSummary", "Select Yes or No");
                }

                return Page();
            }

            BackLink = _urlHelper.Generate(NavigationSection.Home, "Home");
            SubmittedPreferences = true;

            try
            {
                var apprenticePreferencesCommand = new UpdateApprenticePreferencesCommand()
                    { ApprenticePreferences = new List<UpdateApprenticePreferenceCommand>() };
                foreach (ApprenticePreferenceCombination apprenticePreferences in ApprenticePreferences)
                {
                    var apprenticePreference = new UpdateApprenticePreferenceCommand()
                    {
                        ApprenticeId = user.ApprenticeId,
                        PreferenceId = apprenticePreferences.PreferenceId,
                        Status = (bool)apprenticePreferences.Status
                    };
                    apprenticePreferencesCommand.ApprenticePreferences.Add(apprenticePreference);
                }

                await _apprenticeApi.UpdateApprenticePreferences(apprenticePreferencesCommand);

                return Page();
            }
            catch
            {
                return Redirect("Error");
            }
        }
    }
}

