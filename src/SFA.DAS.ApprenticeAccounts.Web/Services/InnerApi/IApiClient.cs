using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using RestEase;

namespace SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi
{
    public interface IApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Post("/apprentices")]
        Task CreateApprenticeAccount([Body] Apprentice apprentice);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);

    }
}