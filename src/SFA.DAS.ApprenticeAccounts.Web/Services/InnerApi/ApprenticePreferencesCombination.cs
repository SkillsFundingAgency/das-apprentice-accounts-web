using System;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class ApprenticePreferencesCombination
    {
        public int PreferenceId { get; set; }
        public string PreferenceMeaning { get; set; }
        public bool? Status { get; set; }
    }
}