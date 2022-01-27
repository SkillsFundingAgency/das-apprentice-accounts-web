using System;

namespace SFA.DAS.ApprenticePortal.OuterApi.Mock.Models
{
    public static class An
    {
        public static Apprentice Apprentice => new Apprentice(
            Guid.NewGuid(),
            Faker.Name.First(),
            Faker.Name.Last(),
            true);
    }
}