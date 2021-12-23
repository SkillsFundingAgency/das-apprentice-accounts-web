using SFA.DAS.ApprenticeAccounts.Web.UnitTests.Hooks;

namespace SFA.DAS.ApprenticeAccounts.Web.UnitTests.FeatureSteps
{
    public class StepsBase
    {
        public StepsBase(TestContext testContext)
        {
            var hook = testContext.Web.ActionResultHook;
            if (hook != null && testContext.ActionResult == null)
            {
                testContext.ActionResult = new TestActionResult();
                hook.OnProcessed = (actionResult) => { testContext.ActionResult.SetActionResult(actionResult); };
                hook.OnErrored = (ex, actionResult) => { testContext.ActionResult.SetException(ex); };
            }
        }
    }
}