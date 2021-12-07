using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    [Binding]
    [Scope(Feature = "ChangeEmailAddress")]
    public class ChangeEmailAddressSteps
    {
        private readonly TestContext _context;
        private readonly AuthenticatedUserContext _userContext;
        private string _link;
        private readonly Guid _clientId = Guid.NewGuid();
        private const string Email = "email";
        private const string Token = "token";

        public ChangeEmailAddressSteps(TestContext context, AuthenticatedUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.UserLoggedIn(_userContext.ApprenticeId);
        }

        [Then(@"the result should redirect to the identity server's change email page")]
        public void ThenTheResultShouldRedirectToTheIdentityServerS()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("https://identity/changeemail");
        }

        [Given(@"they have received the link to change their email address")]
        public void GivenTheyHaveReceivedTheLinkToChangeTheirEmailAddress()
        {
            _link = $"/profile/{_clientId}/changeemail/confirm?email={Email}&token={Token}";
        }

        [When(@"they click on this link")]
        public async Task WhenTheyClickOnThisLink()
        {
            await _context.Web.Get(_link);
        }

        [Then(@"they should be redirected to the login service confirm new email page")]
        public void ThenTheyShouldBeRedirectedToTheLoginServiceConfirmPage()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be($"https://identity/profile/{_clientId}/changeemail/confirm?email={Email}&token={Token}");
        }
    }
}