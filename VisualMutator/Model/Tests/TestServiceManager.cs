namespace VisualMutator.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Services;
    using Strilanc.Value;

    public class TestServiceManager
    {
        private readonly Dictionary<string, ITestsService> _services;

        public TestServiceManager(
            MsTestService msTest,
            NUnitXmlTestService nunit,
            XUnitTestService xunit)
        {
            _services = new List<ITestsService>
                        {
                            msTest,
                            nunit,
                            xunit
                        }.ToDictionary(s => s.FrameWorkName);
        }

        public async Task<List<TestsLoadContext>> LoadTests(IEnumerable<string> assemblyPaths)
        {
            var r = await Task.WhenAll(_services.Values.SelectMany(s => assemblyPaths.Select(assemblyPath =>  Task.Run(() => s.LoadTests(assemblyPath)))));
            return r.Where(m => m.HasValue).Select(m => m.ForceGetValue()).ToList();
        }

        public ITestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath)
        {
            return _services[loadContext.FrameworkName].CreateRunContext(loadContext, mutatedPath);
        }
    }
}