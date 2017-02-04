using NUnit.Framework;

namespace SampleLogic.Tests
{
    [TestFixture()]
    public class SampleClass2BTests
    {
        [Test()]
        public void MethodReturningTrueTest()
        {
            Assert.IsTrue(new SampleClass2B().MethodReturningTrue());
        }

        [Test()]
        public void MethodReturningFalseTest()
        {
            Assert.IsFalse(new SampleClass2B().MethodReturningFalse());
        }

        [Test()]
        public void MethodReturningNullTest()
        {
            Assert.IsNull(new SampleClass2B().MethodReturningNull());
        }
    }
}