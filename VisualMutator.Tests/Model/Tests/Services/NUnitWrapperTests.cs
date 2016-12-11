using System;
using NUnit.Framework;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture()]
    public class NUnitWrapperTests
    {
        [Test()]
        public void NUnitWrapperTest_Constructor_NoException()
        {
            new NUnitWrapper();
        }

        [Test()]
        public void LoadTestsTest()
        {
            var subject = new NUnitWrapper();

            subject.LoadTests(new string[] { @"D:\ovs\Codility\PassingCarsTests3\bin\Debug\PassingCarsTests3.dll" });
        }

        [Test()]
        public void UnloadProjectTest()
        {
            throw new NotImplementedException();
        }
    }
}