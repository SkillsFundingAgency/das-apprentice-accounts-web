using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ApprenticeAccounts.Authentication
{
    public class AuthenticationEvents : OpenIdConnectEvents
    {
        private readonly IApprenticeAccountProvider _provider;

        public AuthenticationEvents(IApprenticeAccountProvider provider)
        {
            _provider = provider;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await base.TokenValidated(context);
            ConvertRegistrationIdToApprenticeId(context.Principal);
            await AddClaims(context.Principal);
        }

        public void ConvertRegistrationIdToApprenticeId(ClaimsPrincipal principal)
        {
            var registrationClaim = principal.Claims.FirstOrDefault(c => c.Type == "registration_id");
            var apprenticeClaim = principal.ApprenticeIdClaim();

            if (registrationClaim == null) return;
            if (apprenticeClaim != null) return;

            principal.AddApprenticeIdClaim(registrationClaim.Value);
        }

        public async Task AddClaims(ClaimsPrincipal principal)
        {
            var apprentice = await GetApprentice(principal);
            if (apprentice == null) 
                return;

            AddApprenticeAccountClaims(principal, apprentice);
        }

        private async Task<IApprenticeAccount> GetApprentice(ClaimsPrincipal principal)
        {
            var claim = principal.ApprenticeIdClaim();

            if (Guid.TryParse(claim?.Value, out var apprenticeId))
                try
                {
                    return await _provider.GetApprenticeAccount(apprenticeId);
                }
                catch
                {
                    return null;
                }

            return null;
        }

        private static void AddApprenticeAccountClaims(ClaimsPrincipal principal, IApprenticeAccount apprentice)
        {
            principal.AddAccountCreatedClaim();
            principal.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(IdentityClaims.GivenName, apprentice.FirstName),
                new Claim(IdentityClaims.FamilyName, apprentice.LastName),
            }));

            if (apprentice.TermsOfUseAccepted)
                principal.AddTermsOfUseAcceptedClaim();
        }

        public static async Task UserAccountCreated(HttpContext context, IApprenticeAccount apprentice)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                AddApprenticeAccountClaims(authenticated.Principal, apprentice);
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }

        public static async Task TermsOfUseAccepted(HttpContext context)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                authenticated.Principal.AddTermsOfUseAcceptedClaim();
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }
    }
}