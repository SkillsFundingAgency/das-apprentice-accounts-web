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

        [BindProperty] public List<ApprenticePreferencesCombination> ApprenticePreferences { get; set; }
        [BindProperty] public string Backlink { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            var preferences = _apprenticeApi.GetPreferences().Result;
            var apprenticePreferences = _apprenticeApi.GetApprenticePreferences(user.ApprenticeId).Result;


            var combination = new List<ApprenticePreferencesCombination>();

            foreach (Preference preference in preferences)
            {
                var apprenticePreference =
                    apprenticePreferences.ApprenticePreferences.Find(a => a.PreferenceId == preference.PreferenceId);
                if (apprenticePreference == null)
                {
                    var tempObject = new ApprenticePreferencesCombination()
                    {
                        PreferenceId = preference.PreferenceId,
                        PreferenceMeaning = preference.PreferenceMeaning,
                        Status = null
                    };
                    combination.Add(tempObject);
                }
                else
                {
                    var tempObject = new ApprenticePreferencesCombination()
                    {
                        PreferenceId = preference.PreferenceId,
                        PreferenceMeaning = preference.PreferenceMeaning,
                        Status = apprenticePreference.Status
                    };
                    combination.Add(tempObject);
                }

                ApprenticePreferences = combination;
            }

            Backlink = _urlHelper.Generate(NavigationSection.Home, "Home");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.ErrorCount > 1)
            {
                ModelState.AddModelError("MultipleErrorSummary", "Select Yes or No");
            }

            return Page();
        }
    }
}

