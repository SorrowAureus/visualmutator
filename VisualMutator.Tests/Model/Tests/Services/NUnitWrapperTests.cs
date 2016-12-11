using NUnit.Framework;
using VisualMutator.Model.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        [Test()]
        public void UnloadProjectTest()
        {
            throw new NotImplementedException();
        }
    }
}