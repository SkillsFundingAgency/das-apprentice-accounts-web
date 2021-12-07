using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticePortal.Authentication.UnitTests.AutoFixtureCustomisations;

namespace SFA.DAS.ApprenticePortal.Authentication.UnitTests
{
    public class AccountCreatedClaimTests
    {
        [Test, MoqAutoData]
        public async Task Users_without_an_apprentice_id_do_not_have_AccountCreated_claim(AuthenticationEvents sut, ClaimsPrincipal identity)
        {
            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("TermsOfUse", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_with_an_invalid_apprentice_id_do_not_have_AccountCreated_claim(AuthenticationEvents sut, ClaimsPrincipal identity, string notAGuid)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(notAGuid));

            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("TermsOfUse", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_not_created_an_account_do_not_have_AccountCreated_claim([Frozen] IApprenticeAccountProvider accountProvider, AuthenticationEvents sut, ClaimsPrincipal identity)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(Guid.NewGuid()));
            Mock.Get(accountProvider).Setup(x => x.GetApprenticeAccount(It.IsAny<Guid>()))
                .ReturnsAsync((IApprenticeAccount)null);

            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("AccountCreated", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("GivenName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("FamilyName", StringComparison.OrdinalIgnoreCase));
            identity.Claims.Should().NotContain(c => c.Type.Equals("TermsOfUse", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_created_an_account_have_AccountCreated_claim_which_is_true([Frozen] IApprenticeAccountProvider accountProvider, AuthenticationEvents sut, Guid apprenticeId, ClaimsPrincipal identity, ApprenticeAccount apprentice)
        {
            identity.AddIdentity(ApprenticeIdClaimsIdentity(apprenticeId));
            Mock.Get(accountProvider).Setup(x => x.GetApprenticeAccount(It.IsAny<Guid>()))
                .ReturnsAsync(apprentice);

            await sut.AddClaims(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "AccountCreated",
                Value = "True",
            });
            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "given_name",
                Value = apprentice.FirstName,
            });
            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "family_name",
                Value = apprentice.LastName,
            });
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_created_but_not_accepted_terms_do_not_have_a_TermsOfUse_claim([Frozen] IApprenticeAccountProvider accountProvider, AuthenticationEvents sut, Guid apprenticeId, ClaimsPrincipal identity, ApprenticeAccount apprentice)
        {
            apprentice.TermsOfUseAccepted = false;
            identity.AddIdentity(ApprenticeIdClaimsIdentity(apprenticeId));
            Mock.Get(accountProvider)
                .Setup(x => x.GetApprenticeAccount(apprenticeId))
                .ReturnsAsync(apprentice);

            await sut.AddClaims(identity);

            identity.Claims.Should().NotContain(c => c.Type.Equals("TermsOfUse", StringComparison.OrdinalIgnoreCase));
        }

        [Test, MoqAutoData]
        public async Task Users_that_have_created_and_accepted_terms_have_a_TermsOfUse_claim([Frozen] IApprenticeAccountProvider accountProvider, AuthenticationEvents sut, Guid apprenticeId, ClaimsPrincipal identity, ApprenticeAccount apprentice)
        {
            apprentice.TermsOfUseAccepted = true;
            identity.AddIdentity(ApprenticeIdClaimsIdentity(apprenticeId));
            Mock.Get(accountProvider)
                .Setup(x => x.GetApprenticeAccount(apprenticeId))
                .ReturnsAsync(apprentice);

            await sut.AddClaims(identity);

            identity.Claims.Should().ContainEquivalentOf(new
            {
                Type = "TermsOfUseAccepted",
                Value = "True",
            });
        }

        private static ClaimsIdentity ApprenticeIdClaimsIdentity(Guid apprenticeId)
            => ApprenticeIdClaimsIdentity(apprenticeId.ToString());

        private static ClaimsIdentity ApprenticeIdClaimsIdentity(string apprenticeId)
            => new ClaimsIdentity(new[] { new Claim("apprentice_id", apprenticeId) });
    }

    public class ApprenticeAccount : IApprenticeAccount
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool TermsOfUseAccepted { get; set; }
    }
}