using SFA.DAS.ApprenticePortal.OuterApi.Mock.Models;
using System;
using System.Linq;

namespace SFA.DAS.ApprenticeAccounts.Web.MockServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            if (args.Contains("--h"))
            {
                Console.WriteLine("Optional parameters (-hasAccount -hasTerms --h)");
                Console.WriteLine("examples:");
                Console.WriteLine("SFA.DAS.ApprenticeAccounts.MockServer --h                 <-- shows this page");
                Console.WriteLine("SFA.DAS.ApprenticeAccounts.MockServer -hasAccount         <-- will return account without terms of use agreed");
                Console.WriteLine("SFA.DAS.ApprenticeAccounts.MockServer -hasTerms           <-- will return account with terms of use agreed");

                Console.WriteLine("");
                Console.WriteLine("");

                return;
            }

            var apiBuilder = new PortalOuterApiMock(5801, true);

            if (args.Contains("-hasAccount"))
            {
                Console.WriteLine("Running with -hasAccount");

                apiBuilder.WithApprentice(An.Apprentice.WithoutTermsOfUseAccepted());
            }
            else if (args.Contains("-hasTerms"))
            {
                Console.WriteLine("Running with -hasTerms");

                apiBuilder.WithApprentice(An.Apprentice.WithTermsOfUseAccepted());
            }
            else
            {
                Console.WriteLine("Running with no account found");
            }

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();
        }
    }
}