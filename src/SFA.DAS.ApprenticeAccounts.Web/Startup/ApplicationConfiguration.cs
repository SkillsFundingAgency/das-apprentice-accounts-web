#nullable disable

using SFA.DAS.ApprenticeCommitments.Web.Startup;
using SFA.DAS.ApprenticePortal.SharedUi;
using SFA.DAS.ApprenticePortal.SharedUi.GoogleAnalytics;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticePortal.SharedUi.Zendesk;

namespace SFA.DAS.ApprenticeAccounts.Web.Startup
{
    public class ApplicationConfiguration : ISharedUiConfiguration
    {
        public AuthenticationServiceConfiguration Authentication { get; set; }
        public DataProtectionConnectionStrings ConnectionStrings { get; set; }
        public InnerApiConfiguration ApprenticeAccountsApi { get; set; }
        public ZenDeskConfiguration Zendesk { get; set; }
        public GoogleAnalyticsConfiguration GoogleAnalytics { get; set; }
        public NavigationSectionUrls ApplicationUrls { get; set; }
    }
}