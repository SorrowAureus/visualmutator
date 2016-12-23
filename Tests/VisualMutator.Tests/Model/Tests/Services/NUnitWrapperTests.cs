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
            new NUnitWrapper(null);
        }

        [Test()]
        public void LoadTestsTest()
        {
            var subject = new NUnitWrapper(null);

            var r = subject.LoadTests(new string[] { @".\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit3Tests\bin\SampleLogicNunit3Tests.dll" });
        }

        [Test()]
        public void UnloadProjectTest()
        {
            throw new NotImplementedException();
        }
    }
}