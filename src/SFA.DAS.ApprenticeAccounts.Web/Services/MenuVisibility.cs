using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.SharedUi.Services;

namespace SFA.DAS.ApprenticeAccounts.Web.Services
{
    internal class MenuVisibility : IMenuVisibility
    {
        public async Task<bool> ShowConfirmMyApprenticeship() => true;

        // The Apprentice Accounts Web does not have an Outer Api
        // Only an Inner Api and we can't determine everything without
        // the CMAD record. So behaving that in Apprentice Accounts you can't
        // See Feedback or the title of Confirm my Apprenticeship option.
        public async Task<bool> ShowApprenticeFeedback() => false;
        public async Task<ConfirmMyApprenticeshipTitleStatus> ConfirmMyApprenticeshipTitleStatus() =>
            ApprenticePortal.SharedUi.Services.ConfirmMyApprenticeshipTitleStatus.DoNotShow;
    }
}