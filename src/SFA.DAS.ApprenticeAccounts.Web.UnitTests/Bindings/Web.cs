using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using SFA.DAS.ApprenticeAccounts.Web.Startup;
using SFA.DAS.ApprenticeAccounts.Web.UnitTests.Hooks;
using SFA.DAS.ApprenticePortal.Authentication.TestHelpers;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests.Bindings
{
    [Binding]
    public class Web
    {
        private Fixture _fixture = new Fixture();
        public static HttpClient Client { get; set; }
        public static CookieContainer Cookies { get; set; }
        public static Dictionary<string, string> Config { get; private set; }
        public static LocalWebApplicationFactory<ApplicationStartup> Factory { get; set; }

        public static Hook<IActionResult> ActionResultHook;

        private readonly TestContext _context;

        public Web(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            if (Client == null)
            {
                Config = new Dictionary<string, string>
                {
                    {"EnvironmentName", "ACCEPTANCE_TESTS"},
                    {"Authentication:MetadataAddress", _context.IdentityServiceUrl},
                    {"ApprenticeAccountsApi:ApiBaseUrl", "https://WWW.GOOGLE.COM" }, // _context.InnerApi.BaseAddress},
                    {"ApplicationUrls:ApprenticeHomeUrl", "https://home/"},
                    {"ApplicationUrls:ApprenticeCommitmentsUrl", "https://confirm/"},
                    {"ApplicationUrls:ApprenticeLoginUrl", "https://login/"},
                    {"ZenDesk:ZendeskSectionId", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskSnippetKey", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskCobrowsingSnippetKey", _fixture.Create<string>()},
                };

                ActionResultHook = new Hook<IActionResult>();
                Factory = new LocalWebApplicationFactory<ApplicationStartup>(Config, ActionResultHook);
                var handler = new CookieContainerHandler()
                {
                    InnerHandler = Factory.Server.CreateHandler(),
                };
                Client = new HttpClient(handler) { BaseAddress = Factory.Server.BaseAddress };
                Cookies = handler.Container;
            }

            _context.Web = new ApprenticeAccountsWeb(Client, ActionResultHook, Config, Cookies);
            AuthenticationHandlerForTesting.Authentications.Clear();
        }

        [AfterScenario()]
        public void CleanUpScenario()
        {
            _context?.Web?.Dispose();
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            Client?.Dispose();
            Factory?.Dispose();
            Client = null;
        }
    }
}