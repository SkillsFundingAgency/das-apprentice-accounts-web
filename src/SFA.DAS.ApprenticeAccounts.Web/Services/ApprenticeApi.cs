using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using SFA.DAS.ApprenticeAccounts.Web.Exceptions;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;

namespace SFA.DAS.ApprenticeAccounts.Web.Services
{
    public class ApprenticeApi
    {
        private readonly IApiClient _client;

        public ApprenticeApi(IApiClient client)
        {
            _client = client;
        }

        public async Task<Apprentice> TryGetApprentice(Guid apprenticeId)
        {
            try
            {
                return await _client.GetApprentice(apprenticeId);
            }
            catch (ApiException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        internal async Task CreateApprentice(Apprentice apprentice)
        {
            await TryApi(() => _client.CreateApprenticeAccount(apprentice));
        }

        internal async Task UpdateApprentice(Guid apprenticeId, string firstName, string lastName, DateTime dateOfBirth)
        {
            await TryApi(async () =>
            {
                var patch = new JsonPatchDocument<Apprentice>()
                    .Replace(x => x.FirstName, firstName)
                    .Replace(x => x.LastName, lastName)
                    .Replace(x => x.DateOfBirth, dateOfBirth);

                await _client.UpdateApprentice(apprenticeId, patch);
            });
        }

        internal async Task AcceptTermsOfUse(Guid apprenticeId)
        {
            await TryApi(async () =>
            {
                await _client.UpdateApprentice(apprenticeId,
                    new JsonPatchDocument<Apprentice>().Replace(x => x.TermsOfUseAccepted, true));
            });
        }

        private async Task TryApi(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                var value = ex.Content!;
                var errors = JsonSerializer.Deserialize<ValidationProblemDetails>(value);
                throw new DomainValidationException(errors);
            }
        }

        internal async Task<List<PreferenceDto>> GetAllPreferences()
        {
            return await _client.GetAllPreferences();
        }

        internal async Task<ApprenticePreferencesDto> GetAllApprenticePreferencesForApprentice(Guid apprenticeId)
        {
            return await _client.GetAllApprenticePreferencesForApprentice(apprenticeId);
        }

        internal async Task<IActionResult> UpdateAllApprenticePreferences(
            ApprenticePreferencesCommand apprenticePreferencesCommand)
        {
            return await _client.UpdateAllApprenticePreferences(apprenticePreferencesCommand);
        }
    }
}