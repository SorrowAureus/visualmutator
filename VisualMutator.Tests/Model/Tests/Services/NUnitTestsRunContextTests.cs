using System;
using NUnit.Framework;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture()]
    public class NUnitTestsRunContextTests : NUnitTestsRunContext
    {
        public NUnitTestsRunContextTests() : base(null, null, null, null, null, null, null)
        {
        }

        [Test()]
        public void PrepareNunitArgsTest()
        {
            var actual = this.PrepareNunitArgs("SomeInputFile", "SomeOutputFile");
            throw new NotImplementedException();
        }
    }
}