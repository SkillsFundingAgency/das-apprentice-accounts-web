using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticePortal.Authentication
{
    public interface IApprenticeAccountProvider
    {
        Task<IApprenticeAccount> GetApprenticeAccount(Guid id);
    }
}