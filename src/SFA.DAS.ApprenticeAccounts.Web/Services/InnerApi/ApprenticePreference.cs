using System;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class ApprenticePreference
    {
        public int PreferenceId { get; set; }
        public string PreferenceMeaning { get; set; }
        public bool Status { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}