namespace VisualMutator.Model.Tests.TestsTree
{
    public class TestNodeClass : TestTreeNode
    {
        public TestNodeClass(string name)
            : base(null, name, true)
        {
        }

        public string FullName { get { return Parent.Name + "." + Name; } }
        public string Namespace { get; set; }
    }
}