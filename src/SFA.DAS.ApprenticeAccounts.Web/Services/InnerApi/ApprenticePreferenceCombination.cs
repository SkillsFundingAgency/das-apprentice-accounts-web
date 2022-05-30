using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public class ApprenticePreferenceCombination
    {
        public int PreferenceId { get; set; }
        public string PreferenceMeaning { get; set; }

        [Required(ErrorMessage = "Select Yes or No")]
        public bool? Status { get; set; }
    }
}