namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using log4net;
    using NUnit.Engine;
    using NUnit.Engine.Services;
    using NUnit.Framework.Interfaces;

    #endregion

    public interface INUnitWrapper
    {
        ITestFilter NameFilter { get; }

        IDictionary<string, List<string>> LoadTests(IEnumerable<string> assemblies);

        void UnloadProject();
    }

    public class NUnitWrapper : INUnitWrapper
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TestEngine engine;

        public ITestFilter NameFilter
        {
            get { return null; }
        }

        public NUnitWrapper()
        {
            engine = new TestEngine();

            engine.InternalTraceLevel = NUnit.Engine.InternalTraceLevel.Off;

            engine.Services.Add(new SettingsService(false));
            engine.Services.Add(new ExtensionService());
            engine.Services.Add(new ProjectService());
            engine.Services.Add(new DomainManager());
            engine.Services.Add(new InProcessTestRunnerFactory());
            engine.Services.Add(new DriverService());

            _log.Info("Initializing Services");
            engine.Initialize();
        }

        public IDictionary<string, List<string>> LoadTests(IEnumerable<string> assemblies)
        {
            var enumerable = assemblies as IList<string> ?? assemblies.ToList();
            _log.Debug("Creating NUnit package for files " + string.Join(", ", enumerable));
            var package = new TestPackage(enumerable.ToList());

            _log.Debug("Loading NUnit package: " + package);

            XmlNode loaded;

            using (var testRunner = engine.GetRunner(package))
            {
                loaded = testRunner.Explore(new NUnit.Engine.TestFilter(""));
                testRunner.Unload();
            }

            var result = new Dictionary<string, List<string>>();

            XElement xml = XElement.Parse(loaded.OuterXml);

            var testFixtures = xml.Descendants("test-suite").Where(p => p.Attributes("type").Single().Value == "TestFixture");

            foreach (var item in testFixtures)
            {
                var testFixtureFullName = item.Attributes("fullname").Single().Value;

                var testCases = item.Descendants("test-case").SelectMany(p => p.Attributes("fullname").Select(q => q.Value)).ToList();

                result.Add(testFixtureFullName, testCases);
            }

            return result;
        }

        public void UnloadProject()
        {
        }
    }
}