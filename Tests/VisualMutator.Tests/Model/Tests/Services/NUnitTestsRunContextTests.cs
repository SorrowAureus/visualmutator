using NUnit.Framework;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture()]
    public class NUnitTestsRunContextTests : NUnitTestsRunContext
    {
        //    var runContext = new NUnitTestsRunContext(
        //options,
        //_kernel.Get<IProcesses>(),
        //_kernel.Get<CommonServices>(),
        //new NUnitResultsParser(),
        //TestProjects.NUnitConsolePath,
        //filePathTests,
        //new TestsSelector());

        public NUnitTestsRunContextTests() : base(new OptionsModel(), null, null, new NUnitResultsParser(), null, null, new TestsSelector())
        {
        }

        [Test()]
        public void PrepareNunitArgsTest()
        {
            var actual = this.PrepareNunitArgs(@"D:\ovs\Codility\PassingCarsTests3\bin\Debug\PassingCarsTests3.dll", @"D:\ovs\Codility\PassingCarsTests3\bin\Debug\result.txt");
        }
    }
}