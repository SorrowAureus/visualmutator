using NUnit.Framework;

namespace SampleLogic.Tests3
{
    [TestFixture()]
    public class SampleClass1Tests
    {
        [Test()]
        public void MethodReturningTrueTest()
        {
            Assert.IsTrue(new SampleClass1().MethodReturningTrue());
        }

        [Test()]
        public void MethodReturningFalseTest()
        {
            Assert.IsFalse(new SampleClass1().MethodReturningFalse());
        }

        [Test()]
        public void MethodReturningNullTest()
        {
            Assert.IsNull(new SampleClass1().MethodReturningNull());
        }
    }
}