using AutoFixture.AutoMoq;

namespace SFA.DAS.ApprenticePortal.Authentication.UnitTests.AutoFixtureCustomisations
{
    public class MoqAutoDataAttribute : AutoDataCustomisationAttributeBase
    {
        public MoqAutoDataAttribute() : base(new AutoMoqCustomization { ConfigureMembers = false })
        {
        }
    }
}