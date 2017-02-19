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

            var result = subject.LoadTests(new string[] { TestProjects.SampleNunit3AssemblyPath });

            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests3.SampleClass1Tests"]);
        }

        [Test()]
        public void LoadTests_loadNunit2Tests_ShowTestsFromNunit2Dll()
        {
            var subject = _kernel.Get<INUnitWrapper>();

            var result = subject.LoadTests(new string[] { TestProjects.SampleNunit2AssemblyPath });

            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests2.SampleClass1Tests"]);
        }

        [Test()]
        public void LoadTests_loadNunit2And3Tests_ShowTestsFromNunit2AndDll()
        {
            var subject = _kernel.Get<INUnitWrapper>();

            var result = subject.LoadTests(new string[] { TestProjects.SampleNunit2AssemblyPath, TestProjects.SampleNunit3AssemblyPath });

            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests2.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests2.SampleClass1Tests"]);

            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests", (ICollection)result.Keys);
            Assert.Contains("SampleLogic.Tests3.SampleClass1Tests.MethodReturningTrueTest", (ICollection)result["SampleLogic.Tests3.SampleClass1Tests"]);
        }
    }
}