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
        public string TestName { get; set; }

        public NUnitTestId(string testName)
        {
            TestName = testName;
        }

        public override string ToString()
        {
            return TestName;
        }
    }

    public class MsTestTestId : TestId
    {
    }
}