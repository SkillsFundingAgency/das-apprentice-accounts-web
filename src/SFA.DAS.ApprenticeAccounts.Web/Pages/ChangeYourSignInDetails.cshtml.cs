using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ApprenticePortal.SharedUi.Filters;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    [HideNavigationBar]
    public class ChangeYourSignInDetailsModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ChangeYourSignInDetailsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [BindProperty]
        public string ChangeEmailLink { get; set; }
        public void OnGet()
        {
            ChangeEmailLink =
                _configuration["ResourceEnvironmentName"]!.Equals("PRD", StringComparison.CurrentCultureIgnoreCase)
                    ? "https://home.account.gov.uk/settings" 
                    : "https://home.integration.account.gov.uk/settings";
        }
    }
}
