using System;
using System.IO;
using System.Reflection;
using Ninject;
using NUnit.Framework;
using UsefulTools.Core;
using VisualMutator.Tests.Util;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture()]
    public class NUnitWrapperTests
    {
        protected StandardKernel _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new IntegrationTestModule());
            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var unitTestAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(unitTestAssemblyDir);
        }

        [Test()]
        public void NUnitWrapperTest_Constructor_NoException()
        {
            new NUnitWrapper(null);
        }

        [Test()]
        public void LoadTestsTest()
        {
            var subject = _kernel.Get<INUnitWrapper>();
            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = TestProjects.NUnitConsoleDirPath;

            var testDllPath = @"D:\github\Visualmutator\visualmutatorpavzaj\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit3Tests\bin\SampleLogicNunit3Tests.dll";

            var r = subject.LoadTests(new string[] { testDllPath });
        }

        [Test()]
        public void UnloadProjectTest()
        {
            throw new NotImplementedException();
        }
    }
}