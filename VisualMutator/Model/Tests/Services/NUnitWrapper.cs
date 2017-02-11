namespace VisualMutator.Model.Tests.Services
{
    using System;

    #region

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using log4net;
    using NUnit.Framework.Interfaces;
    using UsefulTools.Core;

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

        public ITestFilter NameFilter
        {
            get { return null; }
        }

        public ISettingsManager SettingsManager { get; private set; }

        public NUnitWrapper(ISettingsManager settingsManager)
        {
            this.SettingsManager = settingsManager;
        }

        public IDictionary<string, List<string>> LoadTests(IEnumerable<string> assemblies)
        {
            var enumerable = assemblies as IList<string> ?? assemblies.ToList();

            XElement xml = GetNunitTestsFromAssemblies(assemblies);

            var testFixtures = xml.Descendants("test-suite").Where(p => p.Attributes("type").Single().Value == "TestFixture");

            var result = new Dictionary<string, List<string>>();

            foreach (var item in testFixtures)
            {
                var testFixtureFullName = item.Attributes("fullname").Single().Value;

                var testCases = item.Descendants("test-case").SelectMany(p => p.Attributes("fullname").Select(q => q.Value)).ToList();

                result.Add(testFixtureFullName, testCases);
            }

            return result;
        }

        private XElement GetNunitTestsFromAssemblies(IEnumerable<string> assemblies)
        {
            var testAssembliesNunitArgs = string.Join(" ", assemblies);

            _log.Debug("Creating NUnit package for files " + testAssembliesNunitArgs);

            var nUnitConsolePath = Path.GetFullPath(Path.Combine(SettingsManager["NUnitConsoleDirPath"], "nunit3-console.exe"));

            var testExplorationResultFileDir = Path.Combine(Path.GetDirectoryName(nUnitConsolePath), Guid.NewGuid().ToString());

            var startInfo = new ProcessStartInfo
            {
                Arguments = $"--explore:{testExplorationResultFileDir};format=nunit3 --noresult --dispose-runners --skipnontestassemblies {testAssembliesNunitArgs}",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = false,
                RedirectStandardOutput = true,
                FileName = nUnitConsolePath,
                UseShellExecute = false,
            };

            using (var processHandle = Process.Start(startInfo))
            {
                var stdOut = processHandle.StandardOutput.ReadToEnd();
                var timedOut = !processHandle.WaitForExit(20 * 60 * 1000);
                if (timedOut)
                {
                    processHandle.Kill();
                    throw new TimeoutException("Nunit Console timed out!");
                }

                if (stdOut.Contains("NUnitEngineException:"))
                    throw new Exception(stdOut);
            }

            XElement xml = XElement.Parse(File.ReadAllText(testExplorationResultFileDir));

            File.Delete(testExplorationResultFileDir); // how to avoid file operations?

            return xml;
        }

        public void UnloadProject()
        {
        }
    }
}