namespace Afterman.Testing.Tests.Classic
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestBaseSpec
    {
        [TestMethod]
        public void ExecuteValidTestDerivedFromBddTestShouldPass()
        {
            var test = new PassingTest();
            test.Execute(); // expect no exception
        }

        [TestMethod]
        [ExpectedException(typeof(AssertFailedException), "Expected an exception during this test.", AllowDerivedTypes = true)]
        public void ExecuteInvalidTestDerivedFromBddTestShouldFail()
        {
            var test = new FailingTest();
            test.Execute(); // expect an AssertFailedException
        }
    }


}
