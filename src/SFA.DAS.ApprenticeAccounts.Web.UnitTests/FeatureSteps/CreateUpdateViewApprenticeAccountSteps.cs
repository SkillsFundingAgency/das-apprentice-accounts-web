using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeAccounts.Web.Pages;
using SFA.DAS.ApprenticePortal.Authentication.TestHelpers;
using SFA.DAS.ApprenticePortal.OuterApi.Mock.Models;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using WireMock.RequestBuilders;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    [Binding]
    [Scope(Feature = "CreateUpdateViewApprenticeAccount")]
    public class CreateUpdateViewApprenticeAccountSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly AuthenticatedUserContext _userContext;
        private AccountModel _postedApprenticeModel;
        private Apprentice _apprentice;
        private string _registrationCode;

        public CreateUpdateViewApprenticeAccountSteps(TestContext context, AuthenticatedUserContext userContext) : base(context)
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

        [Given("the apprentice has logged in but not created their account")]
        public void GivenTheApprenticeHasLoggedInButNotCreatedTheirAccount()
        {
            _context.UserLoggedIn(_userContext.ApprenticeId);
            GivenTheApprenticeHasNotCreatedTheirAccount();
        }

        [When(@"accessing the account page")]
        public async Task WhenAccessingTheProfilePage()
        {
            await _context.Web.Get("/account");
        }

        [Given("the apprentice has logged in but not accepted the terms of use")]
        public void GivenTheApprenticeHasLoggedInButNotAcceptedTerms()
        {
            _context.Web.AuthoriseApprenticeWithoutTermsOfUse(_userContext.ApprenticeId);
        }

        [Given("the apprentice has not created their account")]
        public void GivenTheApprenticeHasNotCreatedTheirAccount()
        {
            _context.InnerApi.WithoutApprentice(_userContext.ApprenticeId);
        }

        [Given(@"the registration process has been triggered")]
        public void GivenTheRegistrationProcessHasBeenTriggered()
        {
            _registrationCode = Guid.NewGuid().ToString();
            _context.RegistrationCookieIsPresent(_registrationCode);
        }

        [Then("the apprentice should see the personal details page")]
        public void ThenTheApprenticeShouldSeeThePersonalDetailsPage()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            var page = _context.ActionResult.LastPageResult;
            page.Should().NotBeNull();
            page.Model.Should().BeOfType<AccountModel>();
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

        [Then(@"the apprentice should be redirected to Accept Terms of Use Page")]
        public void ThenTheApprenticeShouldBeRedirectedToAcceptTermsOfUsePage()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectToPageResult;
            action.PageName.Should().Be("TermsOfUse");
        }

        [Then(@"the apprentice should be redirected to the login service")]
        public void ThenTheApprenticeShouldBeRedirectedToTheLoginService()
        {
            _context.Web.Response.Should().Be302Redirect();
            var action = _context.ActionResult.LastRedirectResult;
            action.Url.Should().Be("https://confirm/Register");
        }

        [Then(@"the personal details should be empty")]
        public void ThenThePersonalDetailsShouldBeEmpty()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Should().NotBeNull();
            page.Model.Should().BeOfType<AccountModel>()
                .Which.Should().BeEquivalentTo(new
                {
                    FirstName = (string)null,
                    LastName = (string)null,
                    DateOfBirth = (DateModel)null,
                    TermsOfUseAccepted = false,
                    IsCreating = true
                });
        }

        [Then("the apprentice sees their previously entered details")]
        public void ThenTheApprenticeSeesTheirPreviouslyEnteredDetails()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Should().NotBeNull();
            page.Model.Should().BeOfType<AccountModel>()
                .Which.Should().BeEquivalentTo(new
                {
                    _apprentice.FirstName,
                    _apprentice.LastName,
                    DateOfBirth = new
                    {
                        _apprentice.DateOfBirth.Year,
                        _apprentice.DateOfBirth.Month,
                        _apprentice.DateOfBirth.Day,
                    }
                });
        }

        [Given("the apprentice has created their account")]
        public void GivenTheApprenticeHasCreatedTheirAccount()
        {
            _apprentice = An.Apprentice
                .WithId(_userContext.ApprenticeId)
                .WithoutTermsOfUseAccepted();

            _context.InnerApi.WithApprentice(_apprentice);
        }

        [Given("the apprentice has accepted the terms of use")]
        public void GivenTheApprenticeHasAcceptedTheTermsOfUse()
        {
            _apprentice = _apprentice.WithTermsOfUseAccepted();
        }

        [When("the apprentice creates their account with")]
        public async Task WhenTheApprenticeCreatesTheirAccountWith(Table table)
        {
            _postedApprenticeModel = table.CreateInstance(() => new AccountModel(null, null, null, null));
            _postedApprenticeModel.DateOfBirth =
                new DateModel(DateTime.Parse(table.Rows[0]["Date of Birth"]));

            var response = await _context.Web.Post("Account?handler=Register",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "FirstName", _postedApprenticeModel.FirstName },
                    { "LastName", _postedApprenticeModel.LastName },
                    { "DateOfBirth.Day", _postedApprenticeModel.DateOfBirth.Day?.ToString() ?? ""},
                    { "DateOfBirth.Month", _postedApprenticeModel.DateOfBirth.Month?.ToString() ?? "" },
                    { "DateOfBirth.Year", _postedApprenticeModel.DateOfBirth.Year?.ToString() ?? "" },
                }));
        }

        [When("the apprentice updates their account with")]
        public async Task WhenTheApprenticeUpdatesTheirAccountWith(Table table)
        {
            _postedApprenticeModel = table.CreateInstance(() => new AccountModel(null, null, null, null));
            _postedApprenticeModel.DateOfBirth =
                new DateModel(DateTime.Parse(table.Rows[0]["Date of Birth"]));

            var response = await _context.Web.Post("Account",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "FirstName", _postedApprenticeModel.FirstName },
                    { "LastName", _postedApprenticeModel.LastName },
                    { "DateOfBirth.Day", _postedApprenticeModel.DateOfBirth.Day?.ToString() ?? ""},
                    { "DateOfBirth.Month", _postedApprenticeModel.DateOfBirth.Month?.ToString() ?? "" },
                    { "DateOfBirth.Year", _postedApprenticeModel.DateOfBirth.Year?.ToString() ?? "" },
                    { "TermsOfUseAccepted", _apprentice.TermsOfUseAccepted.ToString() },
                }));
        }

        [Then("the apprentice account is updated")]
        public void ThenTheAccountIsUpdated()
        {
            var posts = _context.InnerApi.MockServer.FindLogEntries(
            Request.Create()
                .WithPath("/apprentices*")
                .UsingPatch());

            posts.Should().NotBeEmpty();
        }

        [Then("the apprentice account is created")]
        public void ThenTheVerificationIsSuccessfulSent()
        {
            var posts = _context.InnerApi.MockServer.FindLogEntries(
            Request.Create()
                .WithPath("/apprentices")
                .UsingPost());

            posts.Should().NotBeEmpty();

            var post = posts.First();

            post.RequestMessage.Path.Should().Be("/apprentices");
            var reg = JsonConvert.DeserializeObject<Apprentice>(post.RequestMessage.Body);
            reg.Should().BeEquivalentTo(new
            {
                _userContext.ApprenticeId,
                _postedApprenticeModel.FirstName,
                _postedApprenticeModel.LastName,
                DateOfBirth = _postedApprenticeModel.DateOfBirth.Date,
            });
        }

        [Given("the API will accept the account create")]
        public void WhenTheApiAcceptsTheAccount()
        {
        }

        [Given("the API will accept the account update")]
        public void GivenTheAPIWillAcceptTheAccountUpdate()
        {
        }

        [Given("the API will reject the identity with the following errors")]
        public void WhenTheApiRejectsTheIdentity(Table table)
        {
            var errors = new
            {
                Errors = table.Rows.ToDictionary(
                    row => string.IsNullOrWhiteSpace(row["Property Name"]) ? null : row["Property Name"],
                    row => new[] { row["Error Message"] })
            };

            _context.InnerApi.RejectNewAccounts(errors);
        }

        [Then("the apprentice should see the following error messages")]
        public void ThenTheApprenticeShouldSeeTheFollowingErrorMessages(Table table)
        {
            var messages = table.CreateSet<(string PropertyName, string ErrorMessage)>();
            var lastPage = _context.ActionResult.LastPageResult;

            foreach (var (PropertyName, ErrorMessage) in messages)
            {
                lastPage.Model.As<AccountModel>()
                     .ModelState[PropertyName]
                    .Errors.Should().ContainEquivalentOf(new { ErrorMessage });
            }
        }

        [Then(@"the authentication includes the apprentice's names: ""(.*)"" and ""(.*)""")]
        public void TheAuthenticationIncludesTheApprenticesNames(string firstName, string lastName)
        {
            AuthenticationHandlerForTesting.Authentications.Should().ContainSingle();
            var claims = AuthenticationHandlerForTesting.Authentications[0].Claims;
            claims.Should().ContainEquivalentOf(new
            {
                Type = "given_name",
                Value = firstName,
            });
            claims.Should().ContainEquivalentOf(new
            {
                Type = "family_name",
                Value = lastName,
            });
        }

        const string ReturnUrl = @"https://localhost:7053";

        [Given(@"the query string has return URL")]
        public async Task GivenTheQueryStringHasReturnURL()
        {
            await _context.Web.Get(@$"/account?returnUrl={ReturnUrl}");
        }

        [Then(@"the user is navigated to return URL")]
        public void ThenTheUserIsNavigatedToReturnURL()
        {
            _context.Web.Response.Should().Be302Redirect();
            _context.Web.Response.Headers.Location.Should().Be(ReturnUrl);
        }

    }
}