using NUnit.Framework;
using VisualMutator.Model.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using VisualMutator.Model.Tests.Services;
using VisualMutator.Controllers;
using UsefulTools.Core;
using VisualMutator.Infrastructure;
using VisualMutator.Tests.Util;
using VisualMutator.Model.StoringMutants;
using UsefulTools.ExtensionMethods;
using VisualMutator.Tests.Operators;
using System.IO;
using UsefulTools.Paths;
using VisualMutator.Model.Mutations;
using VisualMutator.Model.Decompilation.CodeDifference;
using VisualMutator.Model.Decompilation;

namespace VisualMutator.Model.Tests.Tests
{
    [TestFixture()]
    public class TestsLoaderTests : IntegrationTest
    {
        [Test()]
        public void LoadTestsTest()
        {
            _kernel.Bind<IProjectClonesManager>().To<ProjectClonesManager>().InSingletonScope();
            _kernel.Bind<ProjectFilesClone>().ToSelf().AndFromFactory();
            _kernel.Bind<FilesManager>().ToSelf().InSingletonScope();
            _kernel.Bind<TestServiceManager>().ToSelf().InSingletonScope();
            _kernel.Bind<XUnitTestService>().ToSelf().InSingletonScope();
            _kernel.Bind<XUnitResultsParser>().ToSelf().InSingletonScope();
            _kernel.Bind<WhiteCache>().ToSelf().AndFromFactory();

            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>().InSingletonScope();
            //_kernel.Bind<OriginalCodebase>().ToConstant(original);
            _kernel.Bind<ICodeDifferenceCreator>().To<CodeDifferenceCreator>().InSingletonScope();
            _kernel.Bind<ICodeVisualizer>().To<CodeVisualizer>().InSingletonScope();
            _kernel.Bind<IMutantsCache>().To<MutantsCache>().InSingletonScope();
            _kernel.Bind<NUnitTestsRunContext>().ToSelf().AndFromFactory();
            _kernel.Bind<XUnitTestsRunContext>().ToSelf().AndFromFactory();
            //_kernel.Bind<OptionsModel>().ToConstant(options);
            _kernel.Bind<IMutationExecutor>().To<MutationExecutor>().InSingletonScope();
            _kernel.Bind<TestingMutant>().ToSelf().AndFromFactory();
            _kernel.Bind<MsTestRunContext>().ToSelf().AndFromFactory();

            _kernel.BindMock<IHostEnviromentConnection>(mock =>
            {
                mock.Setup(_ => _.GetProjectAssemblyPaths()).Returns(new string[] { TestProjects.SomeAnotherTestProjectPath, TestProjects.SampleNunit3AssemblyPath }.Select(p => new FilePathAbsolute(p)));
                mock.Setup(_ => _.GetTempPath()).Returns(Path.GetTempPath());
            });

            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = TestProjects.NUnitConsoleDirPath;

            _kernel.Bind<TestsLoader>().ToSelf().AndFromFactory();

            var testsClone = _kernel.Get<IProjectClonesManager>().CreateClone("Tests");
            var testSubject = _kernel.Get<TestsLoader>();
            var actual = testSubject.LoadTests(testsClone.Assemblies.AsStrings().ToList()).Result;

            Assert.Contains("SampleClass1Tests", actual.Children.Single().Children.Single().Children.Select(p => p.Name).ToArray());
            Assert.Contains("ClassWithConditionalLogicTests", actual.Children.Single().Children.Single().Children.Select(p => p.Name).ToArray());
            Assert.Contains("SampleClass2BTests", actual.Children.Single().Children.Single().Children.Select(p => p.Name).ToArray());
            Assert.Contains("SampleClass2BTests", actual.Children.Single().Children.Single().Children.Select(p => p.Name).ToArray());
        }
    }
}