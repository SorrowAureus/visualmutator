namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using Services;
    using Strilanc.Value;
    using TestsTree;

    public class TestsLoader
    {
        private readonly TestServiceManager _testServiceManager;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TestsLoader(TestServiceManager testServiceManager)
        {
            _testServiceManager = testServiceManager;
        }

        public async Task<TestsRootNode> LoadTests(IList<string> assembliesPaths)
        {
            _log.Info("Loading tests from: " + string.Join(",", assembliesPaths));
            var testsRootNode = new TestsRootNode();

            May<TestNodeAssembly>[] assemblies = await GetTestAssemblyNodes(assembliesPaths, testsRootNode);

            var assembliesNodes = assemblies.WhereHasValue();

            assembliesNodes = assembliesNodes.OrderBy(p => p.Name);

            testsRootNode.Children.AddRange(assembliesNodes);
            testsRootNode.State = TestNodeState.Inactive;
            testsRootNode.IsIncluded = true;
            return testsRootNode;
        }

        private async Task<May<TestNodeAssembly>[]> GetTestAssemblyNodes(IList<string> assembliesPaths, TestsRootNode testsRootNode)
        {
            var tasks = new Dictionary<string, Task<May<TestNodeAssembly>>>();

            foreach (var path in assembliesPaths)
            {
                string path1 = path;
                string assemblyName = Path.GetFileNameWithoutExtension(path);
                var testNodeAssembly = new TestNodeAssembly(testsRootNode, assemblyName);
                testNodeAssembly.AssemblyPath = path;

                var task = LoadFor(path1, testNodeAssembly);
                tasks.Add(path, task);
            }

            var assemblies = await Task.WhenAll(tasks.Values);
            return assemblies;
        }

        private async Task<May<TestNodeAssembly>> LoadFor(string path1, TestNodeAssembly testNodeAssembly)
        {
            var contexts = await _testServiceManager.LoadTests(path1);

            if (contexts.Count == 0)
            {
                return May.NoValue;
            }
            else
            {
                testNodeAssembly.TestsLoadContexts = contexts;

                var allClassNodes = contexts.SelectMany(context => context.ClassNodes);

                IEnumerable<TestNodeNamespace> testNamespaces = TestsLoadContext.GroupTestClasses(allClassNodes.ToList(), testNodeAssembly);

                testNodeAssembly.Children.AddRange(testNamespaces);
                return new May<TestNodeAssembly>(testNodeAssembly);
            }
        }
    }
}