using System;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class UpdateApprenticePreferenceCommand
    {
        public Guid ApprenticeId { get; set; }
        public int PreferenceId { get; set; }
        public bool? Status { get; set; }
    }
}
