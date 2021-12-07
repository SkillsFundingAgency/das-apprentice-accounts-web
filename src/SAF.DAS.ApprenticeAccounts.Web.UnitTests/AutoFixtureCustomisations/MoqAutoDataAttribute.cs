using AutoFixture.AutoMoq;

namespace SAF.DAS.ApprenticeAccounts.Web.UnitTests.AutoFixtureCustomisations
{
    public class MoqAutoDataAttribute : AutoDataCustomisationAttributeBase
    {
        public MoqAutoDataAttribute() : base(new AutoMoqCustomization { ConfigureMembers = false })
        {
        }
    }
}