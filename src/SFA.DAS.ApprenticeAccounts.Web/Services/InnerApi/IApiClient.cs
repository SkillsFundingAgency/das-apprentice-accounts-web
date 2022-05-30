using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        [Get("/preferences")]
        Task<List<Preference>> GetPreferences();

        [Get("/apprenticepreferences/{apprenticeId}")]
        Task<ApprenticePreferencesResponse> GetApprenticePreferences([Path] Guid apprenticeId);

        [Post("/apprenticepreferences/{apprenticeId}/{preferenceId}/{status}")]
        Task<IActionResult> UpdateApprenticePreferences([Path] Guid apprenticeId, [Path] int preferenceId, [Path] bool status);

    }
}