using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeAccounts.Web.Exceptions;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    [HideNavigationBar]
    public class AccountModel : PageModel
    {
        const string ReturnUrlKey = "returnUrl";
        private readonly ApprenticeApi _apprentices;
        private readonly NavigationUrlHelper _urlHelper;

        public AccountModel(ApprenticeApi api, NavigationUrlHelper urlHelper)
        {
            _apprentices = api;
            _urlHelper = urlHelper;
        }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public DateModel? DateOfBirth { get; set; }

        [BindProperty]
        public bool TermsOfUseAccepted { get; set; }

        public string FormHandler => IsCreating ? "Register" : "";

        public bool IsCreating { get; private set; } = false;

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user,
            [FromQuery] string returnUrl)
        {
            ViewData.SetWelcomeText("Welcome");
            if (!string.IsNullOrEmpty(returnUrl)) TempData[ReturnUrlKey] = returnUrl;
            var apprentice = await _apprentices.TryGetApprentice(user.ApprenticeId);

            if (apprentice == null)
            {
                IsCreating = true;
            }
            else
            {
                FirstName = apprentice.FirstName;
                LastName = apprentice.LastName;
                DateOfBirth = apprentice.DateOfBirth == null ? null : new DateModel(apprentice.DateOfBirth.Value);
                TermsOfUseAccepted = apprentice.TermsOfUseAccepted;
            }

            return Page();
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser user)
        {
            try
            {

                var apprentice = new Apprentice
                {
                    ApprenticeId = user.ApprenticeId,
                    FirstName = FirstName,
                    LastName = LastName,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    Email = user.Email.ToString(),
                };

                await _apprentices.UpdateApprentice(user.ApprenticeId, FirstName, LastName, DateOfBirth.Date);
                await AuthenticationEvents.UserAccountUpdated(HttpContext, apprentice);

                return RedirectAfterUpdate();
            }
            catch (DomainValidationException exception)
            {
                foreach (var error in exception.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return Page();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IActionResult> OnPostRegister([FromServices] AuthenticatedUser user)
        {
            IsCreating = true;

            try
            {
                var apprentice = new Apprentice
                {
                    ApprenticeId = user.ApprenticeId,
                    FirstName = FirstName,
                    LastName = LastName,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    Email = user.Email.ToString(),
                };
                await _apprentices.CreateApprentice(apprentice);

                await AuthenticationEvents.UserAccountCreated(HttpContext, apprentice);

                return RedirectAfterUpdate();
            }
            catch (DomainValidationException exception)
            {
                foreach (var error in exception.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return Page();
            }
        }

        private IActionResult RedirectAfterUpdate()
        {
            if (TempData.TryGetValue(ReturnUrlKey, out object returnUrl))
            {
                return Redirect(returnUrl.ToString());
            }
            if (!TermsOfUseAccepted)
                return RedirectToPage("TermsOfUse");

            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return Redirect(_urlHelper.Generate(NavigationSection.Registration));

            return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));
        }
    }
}