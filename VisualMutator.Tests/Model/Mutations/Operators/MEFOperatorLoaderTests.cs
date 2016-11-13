using NUnit.Framework;

namespace VisualMutator.Model.Mutations.Operators.Tests
{
    [TestFixture()]
    public class MEFOperatorLoaderTests
    {
        [Test()]
        public void ReloadOperatorsTest()
        {
            var subject = new MEFOperatorLoader();

            var result = subject.ReloadOperators();
        }
    }
}