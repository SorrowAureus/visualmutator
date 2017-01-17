using System.Reflection;
using Ninject;
using NUnit.Framework;
using SoftwareApproach.TestingExtensions;
using Strilanc.Value;
using UsefulTools.Core;
using VisualMutator.Controllers;
using VisualMutator.Infrastructure;
using VisualMutator.Tests.Util;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture(),Explicit]
    public class NUnitXmlTestServiceTests : IntegrationTest
    {
        [Test()]
        public void LoadTestsTest()
        {
            _kernel.Bind<NUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<NUnitXmlTestService>().ToSelf();
            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();
            _kernel.Bind<MainController>().ToSelf().AndFromFactory();
            _kernel.Bind<IOptionsManager>().To<OptionsManager>().InSingletonScope();
            _kernel.Bind<OptionsController>().ToSelf().AndFromFactory();
            _kernel.Bind<ApplicationController>().ToSelf().InSingletonScope();

            //_kernel.Get<ApplicationController>().Initialize();

            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = Assembly.GetExecutingAssembly().Location + @"\..\..\..\..\packages\NUnit.ConsoleRunner.3.5.0\tools";

            var service = _kernel.Get<NUnitXmlTestService>();

            var loadCtx = service.LoadTests(@"D:\ovs\Codility\PassingCarsTests3\bin\Debug\PassingCarsTests3.dll").ForceGetValue();

            foreach (var ns in loadCtx.Namespaces)
            {
                ns.IsIncluded = true;
            }

            loadCtx.ClassNodes.Count.ShouldBeGreaterThan(0);

            var runCtx = service.CreateRunContext(loadCtx, TestProjects.AutoMapper);
            var results = runCtx.RunTests().Result;
            results.ResultMethods.Count.ShouldBeGreaterThan(0);
        }
    }
}