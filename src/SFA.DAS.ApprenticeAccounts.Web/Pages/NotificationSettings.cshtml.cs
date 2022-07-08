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
                var preferencesDto =  _apprenticeApi.GetAllPreferences();
                var apprenticePreferencesDto = _apprenticeApi.GetAllApprenticePreferencesForApprentice(user.ApprenticeId);
                await Task.WhenAll(preferencesDto, apprenticePreferencesDto);

                var apprenticePreferencesCombination = new List<ApprenticePreference>();

                foreach (var apprenticePreference in from preference in preferencesDto.Result let apprenticePreferenceDto = apprenticePreferencesDto.Result.ApprenticePreferences.FirstOrDefault(ap =>
                             ap.PreferenceId == preference.PreferenceId) select new ApprenticePreference
                         {
                             PreferenceId = preference.PreferenceId,
                             PreferenceMeaning = preference.PreferenceMeaning,
                             Status = apprenticePreferenceDto?.Status
                         })
                {
                    apprenticePreferencesCombination.Add(apprenticePreference);

                    ApprenticePreferences = apprenticePreferencesCombination;
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
                if (ModelState.ErrorCount > 0) ModelState.AddModelError("MultipleErrorSummary", "Select Yes or No");

                return Page();
            }

            BackLink = _urlHelper.Generate(NavigationSection.Home, "Home");
            SubmittedPreferences = true;

            try
            {
                var apprenticePreferencesCommand = new ApprenticePreferencesCommand
                    { ApprenticePreferences = new List<ApprenticePreferenceCommand>() };

                foreach (var apprenticePreference in ApprenticePreferences.Select(apprenticePreferences =>
                             new ApprenticePreferenceCommand
                             {
                                 ApprenticeId = user.ApprenticeId,
                                 PreferenceId = apprenticePreferences.PreferenceId,
                                 Status = apprenticePreferences.Status
                             }))
                    apprenticePreferencesCommand.ApprenticePreferences.Add(apprenticePreference);

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