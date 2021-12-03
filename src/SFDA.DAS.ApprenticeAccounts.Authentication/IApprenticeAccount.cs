namespace SFA.DAS.ApprenticeAccounts.Authentication
{
    public interface IApprenticeAccount
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool TermsOfUseAccepted { get; set; }
    }
}