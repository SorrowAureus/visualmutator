using NUnit.Framework;

namespace SampleLogic.Tests2
{
    [TestFixture()]
    public class SampleClass1Tests
    {
        [Test()]
        public void MethodReturningTrueTest()
        {
            Assert.IsTrue(new SampleClass1().MethodReturningTrue());
        }
    }
}