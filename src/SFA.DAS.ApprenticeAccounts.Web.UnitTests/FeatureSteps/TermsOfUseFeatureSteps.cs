using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeAccounts.Web.Pages;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using SFA.DAS.ApprenticePortal.Authentication.TestHelpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    [Binding]
    [Scope(Feature = "AcceptTermsOfUse")]
    public class TermsOfUseFeatureSteps : StepsBase
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly TestContext _context;
        private readonly AuthenticatedUserContext _userContext;
        private string _registrationCode;

        public TermsOfUseFeatureSteps(TestContext context, AuthenticatedUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _context.ClearCookies();
            _context.InnerApi.Reset();
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.UserLoggedIn(_userContext.ApprenticeId);
        }

        [Given("the apprentice has an account")]
        public void GivenTheApprenticeHasAnAccount()
        {
            AddApprenticeAccount(termsOfUseAccepted: false, reacceptTermsOfUseRequired: false);
        }

        [Given("the apprentice has an account with terms of use accepted")]
        public void GivenTheApprenticeHasAnAccountWithTermsOfUseAccepted()
        {
            AddApprenticeAccount(termsOfUseAccepted: true, reacceptTermsOfUseRequired: false);
        }

        [Given("the apprentice has an account with terms of use needing updating")]
        public void GivenTheApprenticeHasAnAccountWithTermsOfUseNeedingUpdating()
        {
            AddApprenticeAccount(termsOfUseAccepted: true, reacceptTermsOfUseRequired: true);
        }

        private void AddApprenticeAccount(bool termsOfUseAccepted, bool reacceptTermsOfUseRequired)
        {
            var apprentice = _fixture
                .Build<Apprentice>()
                .With(a => a.TermsOfUseAccepted, termsOfUseAccepted)
                .With(b => b.ReacceptTermsOfUseRequired, reacceptTermsOfUseRequired)
                .Create();

            _context.InnerApi.MockServer
                .Given(Request.Create()
                    .WithPath("/apprentices/*").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyAsJson(apprentice));
        }

        [When(@"accessing the terms of use page")]
        public async Task WhenAccessingTheTermsOfUsePage()
        {
            await _context.Web.Get("/termsofuse");
        }

        [Then("the apprentice is shown the Terms of Use")]
        public void ThenTheApprenticeShouldSeeTheTermsOfUse()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            var page = _context.ActionResult.LastPageResult;
            page.Should().NotBeNull();
            page.Model.Should().BeOfType<TermsOfUseModel>();
        }

        [Then(@"the apprentice should be sent to Accept Terms of Use Page")]
        public void ThenTheApprenticeShouldBeSentToAcceptTermsOfUsePage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectToPageResult;
            action.PageName.Should().Be("TermsOfUse");
        }

        [Then(@"the apprentice should be sent to the home page")]
        public void ThenTheApprenticeShouldBeSentToTheHomePage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectResult;
            action.Url.Should().Be("https://home/Home");
        }

        [Then(@"the apprentice should be sent to the registration confirmation page")]
        public void ThenTheApprenticeShouldBeSentToTheRegistrationConfirmationPage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectResult;
            action.Url.Should().Be("https://confirm/Register");
        }

        [Given("the API will accept the confirmation")]
        public void GivenTheAPIWillAcceptTheAccountUpdate()
        {
            _context.InnerApi.MockServer.Given(
                Request.Create()
                    .UsingPatch()
                    .WithPath("/apprentices/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [Then("the authentication includes the terms of use")]
        public void TheAuthenticationIncludesTheTermsOfUse()
        {
            AuthenticationHandlerForTesting.Authentications.Should().ContainSingle();
            var claims = AuthenticationHandlerForTesting.Authentications[0].Claims;
            claims.Should().ContainEquivalentOf(new
            {
                Type = "TermsOfUseAccepted",
                Value = "True",
            });
        }

        [Given(@"the registration process has been triggered")]
        public void GivenTheRegistrationProcessHasBeenTriggered()
        {
            _registrationCode = Guid.NewGuid().ToString();
            _context.RegistrationCookieIsPresent(_registrationCode);
        }

        [When("the apprentice accepts the terms of use")]
        public async Task WhenTheApprenticeAcceptsTheTermsOfUse()
        {
            await _context.Web.Post("TermsOfUse",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "ApprenticeId", Guid.NewGuid().ToString() },
                    { "TermsOfUseAccepted", "true" },
                }));
        }

        [Then(@"the apprentice should be directed to the home page")]
        public void ThenTheApprenticeShouldBeDirectedToTheHomePage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectResult;
            action.Url.Should().Be("https://home/Home");
        }

        [When(@"the apprentice should be sent to the registration confirmation page")]
        public void WhenTheApprenticeShouldBeSentToTheRegistrationConfirmationPage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectResult;
            action.Url.Should().Be("https://confirm/Register");
        }

        [Then("the apprentice account is updated")]
        public void ThenTheAccountIsUpdated()
        {
            var posts = _context.InnerApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}")
                    .UsingPatch());

            posts.Should().NotBeEmpty();
        }

        [Then("is not able to accept the Terms of Use")]
        public void ThenIsNotAbleToAcceptTheTermsOfUse()
        {
            _context.ActionResult.LastPageResult.Model
                .Should().BeOfType<TermsOfUseModel>()
                .Which.PresentAgreement.Should().BeFalse();
        }

        [Then("is able to accept the Terms of Use")]
        public void ThenIsAbleToAcceptTheTermsOfUse()
        {
            _context.ActionResult.LastPageResult.Model
                .Should().BeOfType<TermsOfUseModel>()
                .Which.PresentAgreement.Should().BeTrue();
        }
    }
}