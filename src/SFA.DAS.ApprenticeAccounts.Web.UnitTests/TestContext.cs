using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using SFA.DAS.ApprenticeAccounts.Web.MockServer;
using SFA.DAS.ApprenticeAccounts.Web.UnitTests.Hooks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests
{
    public class TestContext
    {
        private readonly FeatureContext _feature;

        public TestContext(FeatureContext feature)
        {
            _feature = feature;
        }

        public ApprenticeAccountsWeb Web { get; set; }
        public PortalOuterApiMock InnerApi => _feature.GetOrAdd<PortalOuterApiMock>();
        public TestActionResult ActionResult { get; set; }
        public string IdentityServiceUrl { get; } = "https://identity";

        public void ClearCookies()
        {
            Web.Cookies.GetCookies(Web.BaseAddress).ToList().ForEach(c => c.Expired = true);
        }

        public void UserLoggedIn(Guid apprenticeId)
        {
            Web.AuthoriseApprentice(apprenticeId);
            Web.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apprenticeId.ToString());
        }

        public void RegistrationCookieIsPresent(string registrationCode)
        {
            Web.Cookies.Add(new Cookie("RegistrationCode", registrationCode) { Domain = Web.BaseAddress.Host });
        }
    }

    public class AuthenticatedUserContext
    {
        public Guid ApprenticeId { get; set; } = Guid.NewGuid();
    }
}