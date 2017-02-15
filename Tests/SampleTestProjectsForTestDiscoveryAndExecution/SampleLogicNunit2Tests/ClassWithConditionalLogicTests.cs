using NUnit.Framework;

namespace SampleLogic.Tests2
{
    [TestFixture()]
    public class ClassWithConditionalLogicTests
    {
        [Test()]
        public void MultiplyInSillyWayTest()
        {
            var subject = new ClassWithConditionalLogic();

            var actual = subject.MultiplyInSillyWay(7, 2);

            Assert.AreEqual(14, actual);
        }
    }
}