using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeAccounts.Web.Services;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;
using SFA.DAS.ApprenticePortal.SharedUi.Services;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using static System.String;

namespace SFA.DAS.ApprenticeAccounts.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddTransient<ApprenticeApi>();
            services.AddTransient<ISimpleUrlHelper, AspNetCoreSimpleUrlHelper>();
            services.AddScoped<ITimeProvider, UtcTimeProvider>();
            services.AddTransient<IMenuVisibility, MenuVisibility>();
            return services;
        }

        public static IServiceCollection AddInnerApi(
            this IServiceCollection services,
            InnerApiConfiguration configuration)
        {

            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            //if (!IsNullOrWhiteSpace(configuration.IdentifierUri))
            //{
                services.AddTransient<IManagedIdentityTokenGenerator, ManagedIdentityTokenGenerator>();
                services.AddTransient<Http.MessageHandlers.ManagedIdentityHeadersHandler>();
            //}
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();

            var builder = services
                .AddRestEaseClient<IApiClient>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>();

            if (!IsNullOrWhiteSpace(configuration.IdentifierUri))
            {
                builder.AddHttpMessageHandler<Http.MessageHandlers.ManagedIdentityHeadersHandler>();
            }

            builder.AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IManagedIdentityClientConfiguration>((_) => configuration);

            return services;
        }
    }

    public class InnerApiConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string IdentifierUri { get; set; } = null!;
    }
}