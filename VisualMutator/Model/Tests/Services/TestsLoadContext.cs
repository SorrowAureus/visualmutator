namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using TestsTree;

    public class TestsLoadContext
    {
        private readonly string _frameworkName;
        private readonly string _AssemblyName;
        private readonly List<TestNodeClass> _classNodes;
        private readonly List<TestNodeNamespace> _namespaces;

        public TestsLoadContext(string frameworkName, string assemblyName, List<TestNodeClass> classNodes)
        {
            this._frameworkName = frameworkName;
            this._AssemblyName = assemblyName;
            _classNodes = classNodes;
            _namespaces = GroupTestClasses(_classNodes).ToList();
        }

        public List<TestNodeClass> ClassNodes
        {
            get { return _classNodes; }
        }

        public string FrameworkName
        {
            get { return _frameworkName; }
        }

        public string AssemblyName
        {
            get { return _AssemblyName; }
        }

        public List<TestNodeNamespace> Namespaces
        {
            get { return _namespaces; }
        }

        public static IEnumerable<TestNodeNamespace> GroupTestClasses(List<TestNodeClass> classNodes, TestNodeAssembly testNodeAssembly = null)
        {
            return classNodes
                .GroupBy(classNode => classNode.Namespace)
                .OrderBy(p => p.Key)
                .Select(group =>
                {
                    var ns = new TestNodeNamespace(testNodeAssembly, @group.Key);

                    foreach (TestNodeClass nodeClass in @group)
                        nodeClass.Parent = ns;

                    ns.Children.AddRange(@group.OrderBy(p => p.Name));

                    return ns;
                });
        }
    }
}