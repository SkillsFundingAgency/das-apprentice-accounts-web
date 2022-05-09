using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    [HideNavigationBar]
    [AllowAnonymous]
    public class TermsOfUseModel : PageModel
    {
        private readonly ApprenticeApi _client;
        private readonly NavigationUrlHelper _urlHelper;

        public bool PresentAgreement { get; set; }

        [BindProperty]
        public bool TermsOfUseAccepted { get; set; }

        public bool TermsOfUsePreviouslyAccepted { get; set; }

        public bool IsPrivateBetaUser { get; set; }

        public TermsOfUseModel(ApprenticeApi client, NavigationUrlHelper urlHelper)
        {
            _client = client;
            _urlHelper = urlHelper;
        }

        public async Task OnGet()
        {
            if(User.Identity.IsAuthenticated)
            {
                var user = new AuthenticatedUser(User);
                var apprentice = await _client.TryGetApprentice(user.ApprenticeId);
                PresentAgreement = apprentice?.TermsOfUseAccepted == false;
                TermsOfUseAccepted = apprentice.TermsOfUseAccepted;
                TermsOfUsePreviouslyAccepted = apprentice.TermsOfUsePreviouslyAccepted;
                IsPrivateBetaUser = apprentice.IsPrivateBetaUser;
            }
            else
            {
                PresentAgreement = false;
            }
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser apprentice)
        {
            if (TermsOfUseAccepted)
            {
                await _client.AcceptTermsOfUse(apprentice.ApprenticeId);
                await AuthenticationEvents.TermsOfUseAccepted(HttpContext);
            }

            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return Redirect(_urlHelper.Generate(NavigationSection.Registration));

            return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));
        }
    }
}