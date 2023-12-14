using SFA.DAS.ApprenticeAccounts.Web.Attributes;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class ApprenticePreference
    {
        public int PreferenceId { get; set; }
        public string PreferenceMeaning { get; set; }
        public string PreferenceHint { get; set; }

        [RequiredWithDynamicMessage("Select Yes or No to", nameof(PreferenceMeaning))]
        public bool? Status { get; set; }
    }
}