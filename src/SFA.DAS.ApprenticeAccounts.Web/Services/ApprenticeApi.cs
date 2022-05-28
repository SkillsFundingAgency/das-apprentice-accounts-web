using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
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
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var value = ex.Content!;
                var errors = JsonConvert.DeserializeObject<ValidationProblemDetails>(value);
                throw new DomainValidationException(errors);
            }
        }

        public async Task<List<Preference>> GetPreferences()
        {
            return await _client.GetPreferences();
        }

        public async Task<ApprenticePreferencesResponse> GetApprenticePreferences(Guid apprenticeId)
        {
            return await _client.GetApprenticePreferences(apprenticeId);
        }
    }
}