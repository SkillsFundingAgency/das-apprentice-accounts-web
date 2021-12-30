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

            var apiBuilder = ApprenticeAccountsApiBuilder.Create(5801);

            if (args.Contains("-hasAccount"))
            {
                Console.WriteLine("Running with -hasAccount");

                apiBuilder = apiBuilder.WithUserAccountButNoTermsOfUseAgreed();
            }
            else if (args.Contains("-hasTerms"))
            {
                Console.WriteLine("Running with -hasTerms");

                apiBuilder = apiBuilder.WithUserAccountAndTermsOfUseAgreed();
            }
            else
            {
                Console.WriteLine("Running with no account found");
            }

            apiBuilder.BuildAndRun();

            Console.WriteLine("Press any key to stop the servers");
            Console.ReadKey();
        }
    }
}