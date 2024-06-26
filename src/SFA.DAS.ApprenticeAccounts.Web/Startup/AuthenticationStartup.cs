using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;

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

            return services;
        }
    }

    public class AuthenticationServiceConfiguration 
    {
        public string MetadataAddress { get; set; } = null!;
        public string ChangeEmailPath { get; set; } = "/changeemail";
    }
}