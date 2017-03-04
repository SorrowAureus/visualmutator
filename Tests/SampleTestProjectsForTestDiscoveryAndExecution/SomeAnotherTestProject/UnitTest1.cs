using NUnit.Framework;

namespace SomeAnotherTestProjectNamespace
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void TestMethod1(int a)
        {
        }
    }
}