using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeAccounts.Authentication
{
    public interface IApprenticeAccountProvider
    {
        Task<IApprenticeAccount> GetApprenticeAccount(Guid id);
    }
}