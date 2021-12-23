using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ApprenticeAccounts.Web.UnitTests.Hooks;
using SFA.DAS.ApprenticePortal.Authentication.TestHelpers;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;
        private readonly IHook<IActionResult> _actionResultHook;

        public LocalWebApplicationFactory(Dictionary<string, string> config, IHook<IActionResult> actionResultHook)
        {
            _config = config;
            _actionResultHook = actionResultHook;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x => x.UseStartup<TEntryPoint>());
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandlerForTesting>("TestScheme", _ => { });
            });

            builder.ConfigureServices(s =>
            {
                s.AddControllersWithViews(options =>
                {
                    options.Filters.Add(new ActionResultFilter(_actionResultHook));
                });

                s.AddMvc().AddRazorPagesOptions(o =>
                {
                    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                });
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment(Environments.Development);
        }
    }
}