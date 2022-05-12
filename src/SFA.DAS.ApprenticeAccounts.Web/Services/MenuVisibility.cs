using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.SharedUi.Services;

namespace SFA.DAS.ApprenticeAccounts.Web.Services
{
    internal class MenuVisibility : IMenuVisibility
    {
        public async Task<bool> ShowConfirmMyApprenticeship() => true;

        public async Task<bool> ShowApprenticeFeedback() => true;
    }
}