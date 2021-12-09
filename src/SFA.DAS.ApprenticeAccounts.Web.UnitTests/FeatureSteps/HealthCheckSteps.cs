using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    [Binding]
    [Scope(Feature = "HealthCheck")]
    public class HealthCheckSteps : StepsBase
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext context) : base(context) => _context = context;

        [Then(@"the result should be ""(.*)""")]
        public void ThenTheResultShouldBe(string expected)
        {
            _context.Web.Response.EnsureSuccessStatusCode();
            _context.Web.Response.Content.ReadAsStringAsync().Result.Should().Be(expected);
        }

        [When(@"accessing the ping endpoint")]
        public async Task WhenAccessingThePingEndpoint()
        {
            await _context.Web.Get("ping");
        }
    }
}