using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticePortal.SharedUi.Startup;

namespace SFA.DAS.ApprenticeAccounts.Web.Startup
{
    public class ApplicationStartup
    {
        public ApplicationStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appConfig = Configuration.Get<ApplicationConfiguration>();

            services
                .AddApplicationInsightsTelemetry()
                .AddDataProtection(appConfig.ConnectionStrings, Environment)
                .AddAuthentication(appConfig.Authentication, Environment)
                .AddInnerApi(appConfig.ApprenticeAccountsApi, Environment)
                .RegisterServices()
                .AddControllers();

            services.AddSharedUi(appConfig, options =>
            {
                options.EnableZendesk();
                options.EnableGoogleAnalytics();
                options.SetCurrentNavigationSection(NavigationSection.ConfirmMyApprenticeship);
            });

            services.AddRazorPages().AddRazorPagesOptions(
                options =>
                {
                    options.Conventions.AddPageRoute("/account", "{*url}");
                });


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseErrorPages(env)
                .UseStatusCodePagesWithReExecute("/Error")
                .UseHsts(env)
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseHealthChecks()
                .UseRouting()
                .UseMiddleware<SecurityHeadersMiddleware>()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                });
        }
    }
}
