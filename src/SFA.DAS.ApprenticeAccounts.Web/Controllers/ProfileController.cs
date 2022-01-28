using System;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeAccounts.Web.Startup;

namespace SFA.DAS.ApprenticeAccounts.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AuthenticationServiceConfiguration _authenticationConfig;

        public ProfileController(AuthenticationServiceConfiguration authenticationConfig)
        {
            _authenticationConfig = authenticationConfig;
        }

        [Authorize]
        [HttpGet("/profile/{clientId}/changeemail/confirm")]
        public IActionResult Confirm([FromRoute] Guid clientId, [FromQuery] string email, [FromQuery] string token)
        {
            var baseUrl = _authenticationConfig.MetadataAddress;
            var endpoint = (baseUrl.EndsWith("/") ? baseUrl : $"{baseUrl}/") +
                       $"profile/{clientId}/changeemail/confirm?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}";
            return Redirect(endpoint);
        }

        [Authorize]
        [HttpGet("/AcceptTermsOfUse")]
        public IActionResult AcceptTermsOfUse()
            => Redirect("/TermsOfUse");
    }
}
