using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly AuthenticatedUser _authenticatedUser;

        public bool PresentAgreement { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "You must accept the terms and conditions")]
        public bool TermsOfUseAccepted { get; set; }

        public bool ReacceptTermsOfUseRequired { get; set; }

        public TermsOfUseModel(ApprenticeApi client, NavigationUrlHelper urlHelper, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _urlHelper = urlHelper;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {           
                if(User.Identity.IsAuthenticated)
            {
                var apprentice = await _client.TryGetApprentice(_authenticatedUser.ApprenticeId);

                ReacceptTermsOfUseRequired = apprentice?.ReacceptTermsOfUseRequired == true;
                // If Reaccept is true, TermsOfUseAccepted is forced to false, so check can just be on TermsOfUse.
                PresentAgreement = apprentice?.TermsOfUseAccepted == false;
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
            } else
            {
                ModelState.AddModelError("TermsOfUseAccepted", "You must accept the terms and conditions");
                return Page();
            }

            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return Redirect(_urlHelper.Generate(NavigationSection.Registration));

            return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));
        }
    }
}