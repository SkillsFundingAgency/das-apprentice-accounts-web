using Newtonsoft.Json;
using System;

namespace SFA.DAS.ApprenticePortal.OuterApi.Mock.Models
{
    public sealed class Apprentice
    {
        public Guid ApprenticeId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public DateTime DateOfBirth { get; set; }
        public bool TermsOfUseAccepted { get; }

        public Apprentice(
            Guid apprenticeId,
            string firstName,
            string lastName,
            bool termsOfUseAccepted)
        {
            ApprenticeId = apprenticeId;
            FirstName = firstName;
            LastName = lastName;
            TermsOfUseAccepted = termsOfUseAccepted;
        }

        public Apprentice WithAnyId() => With(id: Guid.Empty);

        public Apprentice WithId(Guid id) => With(id: id);

        public Apprentice WithFirstName(string firstName) => With(firstName: firstName);

        public Apprentice WithLastName(string lastName) => With(lastName: lastName);

        public Apprentice WithTermsOfUseAccepted()
            => With(termsAccepted: true);

        public Apprentice WithoutTermsOfUseAccepted()
            => With(termsAccepted: false);

        private Apprentice With(
            Guid? id = null,
            string? firstName = null,
            string? lastName = null,
            bool? termsAccepted = null)
            => new Apprentice(
                id ?? ApprenticeId,
                firstName ?? FirstName,
                lastName ?? LastName,
                termsAccepted ?? TermsOfUseAccepted);
    }

    public static class ApprenticeExtensions
    {
        public static string ApprenticeUrlId(this Apprentice apprentice)
            => apprentice.ApprenticeId == Guid.Empty ? "*" : apprentice.ApprenticeId.ToString();
    }
}