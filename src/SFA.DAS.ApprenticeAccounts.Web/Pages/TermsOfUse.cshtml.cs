using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    [HideNavigationBar]
    public class TermsOfUseModel : PageModel
    {
        private readonly AuthenticatedUser _apprentice;
        private readonly ApprenticeApi _client;
        private readonly NavigationUrlHelper _urlHelper;

        [BindProperty]
        public bool TermsOfUseAccepted { get; set; }

        public TermsOfUseModel(AuthenticatedUser apprentice, ApprenticeApi client, NavigationUrlHelper urlHelper)
        {
            _apprentice = apprentice;
            _client = client;
            _urlHelper = urlHelper;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (TermsOfUseAccepted)
            {
                await _client.AcceptTermsOfUse(_apprentice.ApprenticeId);
                await AuthenticationEvents.TermsOfUseAccepted(HttpContext);
            }

            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return Redirect(_urlHelper.Generate(NavigationSection.Registration));

            return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));
        }
    }
}