using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAccounts.Web.Startup
{
    public static class AuthenticationStartup
    {
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            AuthenticationServiceConfiguration config,
            IWebHostEnvironment environment)
        {
            services
                .AddApplicationAuthentication(config, environment)
                .AddApplicationAuthorisation();

            services.AddTransient((_) => config);

            return services;
        }

        private static IServiceCollection AddApplicationAuthentication(
            this IServiceCollection services,
            AuthenticationServiceConfiguration config,
            IWebHostEnvironment environment)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddApprenticeAuthentication(config.MetadataAddress.ToString(), environment);
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            return services;
        }
        
        public static void AddGovLoginAuthentication(
            this IServiceCollection services,
            NavigationSectionUrls config,
            IConfiguration configuration)
        {
            services.AddGovLoginAuthentication(configuration);
            services.AddAuthorization();

            services.AddRazorPages(o => o.Conventions
                .AuthorizeFolder("/")
                .AllowAnonymousToPage("/ping"));
            services.AddScoped<AuthenticatedUser>();
            services.AddHttpContextAccessor();
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            services.AddTransient((_) => config);
            services.AddTransient((_) => new AuthenticationServiceConfiguration());
        }

        private static IServiceCollection AddApplicationAuthorisation(
            this IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddRazorPages(o => o.Conventions
                .AuthorizeFolder("/")
                .AllowAnonymousToPage("/ping"));
            services.AddScoped<AuthenticatedUser>();
            services.AddScoped(s => s
                .GetRequiredService<IHttpContextAccessor>().HttpContext.User);
            services.AddHttpContextAccessor();
            return services;
        }
    }

    public class AuthenticationServiceConfiguration 
    {
        public string MetadataAddress { get; set; } = null!;
        public string ChangeEmailPath { get; set; } = "/changeemail";
    }
}