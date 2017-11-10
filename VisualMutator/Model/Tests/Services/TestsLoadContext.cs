namespace VisualMutator.Model.Tests.Services
{
  using System.Collections.Generic;
  using System.Linq;
  using TestsTree;

  public class TestsLoadContext
  {
    public TestsLoadContext(string frameworkName, List<TestNodeClass> classNodes)
    {
      this.FrameworkName = frameworkName;
      ClassNodes = classNodes;
      Namespaces = GroupTestClasses(ClassNodes).ToList();
    }

    public List<TestNodeClass> ClassNodes { get; }

    public string FrameworkName { get; }

    public List<TestNodeNamespace> Namespaces { get; }

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