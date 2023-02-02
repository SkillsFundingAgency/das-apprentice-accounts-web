using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.ApprenticeAccounts.Web.Startup
{
    public static class ConfigurationStartup
    {
        public static IWebHostBuilder ConfigureAzureTableConfiguration(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.Build();

                if (!config["EnvironmentName"].Equals("DEV", StringComparison.InvariantCultureIgnoreCase))
                {
                    configBuilder.AddAzureTableStorage(options =>
                    {
                        var (names, connectionString, environment) = configBuilder.ConfigurationSections();
                        options.ConfigurationKeys = names.Split(",");
                        options.StorageConnectionString = connectionString;
                        options.EnvironmentName = environment;
                        options.PreFixConfigurationKeys = false;
                    });
                }
                configBuilder.AddJsonFile("appsettings.Development.json", true);
            });

            return hostBuilder;
        }

        private static (string names, string connectionString, string environment) ConfigurationSections(this IConfigurationBuilder configBuilder)
        {
            var config = configBuilder.Build();
            return
                (
                    config["ConfigNames"],
                    config["ConfigurationStorageConnectionString"],
                    config["EnvironmentName"]
                );
        }
    }
}