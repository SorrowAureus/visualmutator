namespace VisualMutator.Model.Tests.Services
{
    using System;

    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using NUnit.Engine;
    using NUnit.Engine.Runners;
    using NUnit.Engine.Services;
    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;

    #endregion

    public interface INUnitWrapper
    {
        ITestFilter NameFilter { get; }

        ITest LoadTests(IEnumerable<string> assemblies);

        void UnloadProject();
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ITestEngine nunitTestEngine;

        public ITestFilter NameFilter
        {
            get { return null; }
        }

        public NUnitWrapper()
        {
            // Create TestEngine - this program is
            // conceptually part of  the engine and
            // can access it's internals as needed.
            TestEngine engine = new TestEngine();

            // TODO: We need to get this from somewhere. Argument?
            engine.InternalTraceLevel = NUnit.Engine.InternalTraceLevel.Debug;

            // Custom Service Initialization
            //log.Info("Adding Services");
            engine.Services.Add(new SettingsService(false));
            engine.Services.Add(new ExtensionService());
            engine.Services.Add(new ProjectService());
            engine.Services.Add(new DomainManager());
            engine.Services.Add(new InProcessTestRunnerFactory());
            engine.Services.Add(new DriverService());
            //engine.Services.Add( new TestLoader() );

            // Initialize Services
            _log.Info("Initializing Services");
            engine.Initialize();

            ////InternalTrace.Initialize("nunit-visual-mutator.log", InternalTraceLevel.Verbose);

            //nunitTestEngine = new TestEngine();

            //// TODO: We need to get this from somewhere. Argument?
            //nunitTestEngine.InternalTraceLevel = NUnit.Engine.InternalTraceLevel.Debug;

            //// Custom Service Initialization
            ////log.Info("Adding Services");
            //((ServiceContext)nunitTestEngine.Services).Add(new SettingsService(false));
            //((ServiceContext)nunitTestEngine.Services).Add(new ExtensionService());
            //((ServiceContext)nunitTestEngine.Services).Add(new ProjectService());
            //((ServiceContext)nunitTestEngine.Services).Add(new DomainManager());
            //((ServiceContext)nunitTestEngine.Services).Add(new InProcessTestRunnerFactory());
            //((ServiceContext)nunitTestEngine.Services).Add(new DriverService());
            ////engine.Services.Add( new TestLoader() );

            //// Initialize Services
            //_log.Info("Initializing Services");
            //nunitTestEngine.Initialize();
        }

        public ITest LoadTests(IEnumerable<string> assemblies)
        {
            var enumerable = assemblies as IList<string> ?? assemblies.ToList();
            _log.Debug("Creating NUnit package for files " + string.Join(", ", enumerable));
            var package = new TestPackage(enumerable.ToList());
            package.Settings["RuntimeFramework"] = new NUnit.Framework.Internal.RuntimeFramework(NUnit.Framework.Internal.RuntimeType.Net, Environment.Version);
            package.Settings["UseThreadedRunner"] = false;

            //                lock (this)
            //                {
            _log.Debug("Loading NUnit package: " + package);

            var testRunner = new LocalTestRunner(this.nunitTestEngine.Services, package);
            var loaded = testRunner.Load();

            var tests = loaded.XmlNodes;
            testRunner.Unload();

            ITest result = new TestSuite("dummy suite");

            foreach (var item in tests)
            {
                result.AddToXml(TNode.FromXml(item.OuterXml), true);
            }
            return result;
            //                }
        }

        public void UnloadProject()
        {
        }
    }
}