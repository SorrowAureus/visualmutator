using VisualMutator.Model.Tests.Services;
using System.Reflection;
using Ninject;
using NUnit.Framework;
using SoftwareApproach.TestingExtensions;
using Strilanc.Value;
using UsefulTools.Core;
using VisualMutator.Controllers;
using VisualMutator.Infrastructure;
using VisualMutator.Tests.Util;
using System.IO;
using System;
using System.Linq;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture]
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

            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = TestProjects.NUnitConsoleDirPath;
            var service = _kernel.Get<NUnitXmlTestService>();

            var testDllPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit3Tests\bin\SampleLogicNunit3Tests.dll"));

            var loadCtx = service.LoadTests(TestProjects.SampleNunit3AssemblyPath).ForceGetValue();

            foreach (var ns in loadCtx.Namespaces)
            {
                ns.IsIncluded = true;
            }

            loadCtx.Namespaces.Count.ShouldBeGreaterThan(0);
            loadCtx.ClassNodes.Count.ShouldBeGreaterThan(0);

            Assert.AreEqual("SampleClass1Tests", loadCtx.ClassNodes.First(p => p.FullName.Contains("SampleClass1Tests")).Name);
            Assert.AreEqual("SampleLogic.Tests3", loadCtx.ClassNodes.First(p => p.FullName.Contains("SampleClass1Tests")).Namespace);

            CollectionAssert.IsOrdered(loadCtx.ClassNodes.Select(p => p.FullName));
            CollectionAssert.IsOrdered(loadCtx.ClassNodes.First(p => p.FullName.Contains("SampleClass1Tests")).Children.Select(p => p.Name));
        }
    }
}