using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class ApprenticePreferencesCommand
    {
        public IEnumerable<ApprenticePreferenceCommand> ApprenticePreferences { get; set; }
        public Guid ApprenticeId { get; set; }
    }
}
