using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Ninject;
using NUnit.Framework;
using UsefulTools.Core;
using VisualMutator.Tests.Util;

namespace VisualMutator.Model.Tests.Services.Tests
{
    [TestFixture]
    public class NUnitWrapperTests
    {
        protected StandardKernel _kernel;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new IntegrationTestModule());
            _kernel.Bind<INUnitWrapper>().To<NUnitWrapper>();
            _kernel.Get<ISettingsManager>()["NUnitConsoleDirPath"] = TestProjects.NUnitConsoleDirPath;
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
        public void LoadTests_loadNunit3Tests_ShowTestsFromNunit3Dll()
        {
            var subject = _kernel.Get<INUnitWrapper>();

            var testDllPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit3Tests\bin\SampleLogicNunit3Tests.dll"));

            var result = subject.LoadTests(new string[] { testDllPath });

            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests3.SampleClass1Tests"]);
        }

        [Test()]
        public void LoadTests_loadNunit2Tests_ShowTestsFromNunit2Dll()
        {
            var subject = _kernel.Get<INUnitWrapper>();

            var testDllPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit2Tests\bin\SampleLogicNunit2Tests.dll"));

            var result = subject.LoadTests(new string[] { testDllPath });

            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests2.SampleClass1Tests"]);
        }

        [Test()]
        public void LoadTests_loadNunit2And3Tests_ShowTestsFromNunit2AndDll()
        {
            var subject = _kernel.Get<INUnitWrapper>();

            var testDllPath2 = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit2Tests\bin\SampleLogicNunit2Tests.dll"));
            var testDllPath3 = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\Tests\SampleTestProjectsForTestDiscoveryAndExecution\SampleLogicNunit3Tests\bin\SampleLogicNunit3Tests.dll"));

            var result = subject.LoadTests(new string[] { testDllPath2, testDllPath3 });

            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests2.SampleClass1Tests"]);

            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests3.SampleClass1Tests"]);
        }
    }
}