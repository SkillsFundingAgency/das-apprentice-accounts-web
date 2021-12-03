using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using SFA.DAS.ApprenticeAccounts.Web.Startup;
using SFA.DAS.ApprenticeCommitments.Web.Startup;

namespace SFA.DAS.ApprenticeAccounts.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            NLogStartup.ConfigureNLog();
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAzureTableConfiguration()
                .UseStartup<ApplicationStartup>()
                .UseNLog();
    }
}