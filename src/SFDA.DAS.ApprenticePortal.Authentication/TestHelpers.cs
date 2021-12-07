using System;
using System.Security.Claims;

namespace SFA.DAS.ApprenticePortal.Authentication
{
    public class TestHelpers
    {
        public static AuthenticatedUser FakeLocalUserFullyVerified => new AuthenticatedUser(FakeLocalUserFullyVerifiedClaim);
        public static AuthenticatedUser FakeLocalUserWithNoAccount => new AuthenticatedUser(FakeLocalUserWithNoAccountClaim);

        public static ClaimsPrincipal FakeLocalUserFullyVerifiedClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new[]
                {
                    new Claim(IdentityClaims.ApprenticeId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.LogonId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.Name, "lisa_simpson_esfa@mailinator.com"),
                    new Claim(IdentityClaims.GivenName, "Lisa"),
                    new Claim(IdentityClaims.FamilyName, "Simpson"),
                    new Claim(IdentityClaims.AccountCreated, "True"),
                    new Claim(IdentityClaims.TermsOfUseAccepted, "True"),
                    new Claim(IdentityClaims.VerifiedUser, "True"),
                })
            });

        public static ClaimsPrincipal FakeLocalUserWithNoAccountClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new[]
                {
                    new Claim(IdentityClaims.ApprenticeId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.LogonId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.Name, "bart_simpson_esfa@mailinator.com"),
                })
            });
    }
}