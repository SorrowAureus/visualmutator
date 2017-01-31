namespace VisualMutator.Model.Tests
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Mutations.MutantsTree;
    using Strilanc.Value;
    using TestsTree;

    #endregion

    public interface ITestsContainer
    {
        void CancelAllTesting();

        IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant);
    }

    public class TestsContainer : ITestsContainer
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TestResultTreeCreator _testResultTreeCreator;

        public TestsContainer()
        {
            _testResultTreeCreator = new TestResultTreeCreator();
        }

        public void CancelAllTesting()
        {
        }

        public IEnumerable<TestNodeNamespace> CreateMutantTestTree(Mutant mutant)
        {
            List<TmpTestNodeMethod> nodeMethods = mutant.TestRunContexts
                .SelectMany(c => c.TestResults.ResultMethods).ToList();

            return _testResultTreeCreator.CreateMutantTestTree(nodeMethods);
        }
    }
}