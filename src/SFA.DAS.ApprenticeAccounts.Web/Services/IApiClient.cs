using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;

namespace SFA.DAS.ApprenticeAccounts.Web.Services
{
    public interface IApiClient
    {
        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Post("/apprentices")]
        Task CreateApprenticeAccount([Body] Apprentice apprentice);
        
        [Put("/apprentices")]
        Task<Apprentice> PutApprentice([Body] PutApprenticeRequest request);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprentice([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);

        [Get("/preferences")]
        Task<List<PreferenceDto>> GetAllPreferences();

        [Get("/apprenticepreferences/{apprenticeId}")]
        Task<ApprenticePreferencesDto> GetAllApprenticePreferencesForApprentice([Path] Guid apprenticeId);

        [Post("/apprenticepreferences/{apprenticeId}")]
        Task<IActionResult> UpdateAllApprenticePreferences([Path] Guid apprenticeId ,[Body] ApprenticePreferencesCommand apprenticePreferencesCommand);

    }
}