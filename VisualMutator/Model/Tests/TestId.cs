using NUnit.Framework.Interfaces;

namespace VisualMutator.Model.Tests
{
    #region

    #endregion

    public abstract class TestId
    {
    }

    public class NUnitTestId : TestId
    {
        public ITest TestName { get; set; }

        public NUnitTestId(ITest testName)
        {
            TestName = testName;
        }

        public override string ToString()
        {
            return TestName.FullName;
        }
    }

    public class MsTestTestId : TestId
    {
    }
}