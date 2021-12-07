using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SAF.DAS.ApprenticeAccounts.Web.UnitTests.Features;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SAF.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    [Binding]
    [Scope(Feature = "Zendesk")]
    public class ZendeskSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly AuthenticatedUserContext _userContext;
        private Guid apprenticeId = Guid.NewGuid();

        public ZendeskSteps(TestContext context, AuthenticatedUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            context.Web.AuthoriseApprentice(apprenticeId);

            SetupCallToGetApprenticeDetails();
        }

        private void SetupCallToGetApprenticeDetails()
        {
            var apprentice = new Apprentice
            {
                ApprenticeId = Guid.NewGuid(),
                Email = "someone@example.com",
                FirstName = "Someone",
                LastName = "Wurst",
                DateOfBirth = new DateTime(2008, 08, 21),
                TermsOfUseAccepted = false,
            };

            _context.InnerApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/{_userContext.ApprenticeId}")
                )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(apprentice));
        }

        [When("a page is requested")]
        public async Task WhenPageIsRequested()
        {
            await _context.Web.Get($"apprentices/{_userContext.ApprenticeId}");
        }

        [Then("the page contains the Zendesk configuration")]
        public async Task ThenThePageContainsTheZendeskConfiguration()
        {
            Assert.That(_context.Web.Response, Is.Not.Null);
            var body = await _context.Web.Response.Content.ReadAsStringAsync();
            Assert.That(body, Contains.Substring($"section: '{_context.Web.Config["ZenDesk:ZendeskSectionId"]}'"));
            Assert.That(body, Contains.Substring(
                $@"<script id=""ze-snippet"" src=""https://static.zdassets.com/ekr/snippet.js?key={_context.Web.Config["ZenDesk:ZendeskSnippetKey"]}"""));
            Assert.That(body, Contains.Substring(
                $@"<script id=""co-snippet"" src=""https://embed-euw1.rcrsv.io/{_context.Web.Config["ZenDesk:ZendeskCobrowsingSnippetKey"]}?zwwi=1"""));
        }
    }
}