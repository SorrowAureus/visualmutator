namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using log4net;
    using NUnit.Engine;
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

            // Initialize Services
            _log.Info("Initializing Services");
            engine.Initialize();
        }

        public ITest LoadTests(IEnumerable<string> assemblies)
        {
            var enumerable = assemblies as IList<string> ?? assemblies.ToList();
            _log.Debug("Creating NUnit package for files " + string.Join(", ", enumerable));
            var package = new TestPackage(enumerable.ToList());

            //                lock (this)
            //                {
            _log.Debug("Loading NUnit package: " + package);

            XmlNode loaded;

            using (var testRunner = engine.GetRunner(package))
            {
                loaded = testRunner.Explore(new NUnit.Engine.TestFilter(""));
  
                testRunner.Unload();
            }

            loaded =

<?xml version="1.0" encoding="UTF-8"?>
<test-run testcasecount="19" id="2">
   <test-suite testcasecount="19" id="0-1021" runstate="Runnable" fullname="D:\\ovs\\Codility\\PassingCarsTests3\\bin\\Debug\\PassingCarsTests3.dll" name="PassingCarsTests3.dll" type="Assembly">
      <properties>
         <property name="_PID" value="10656" />
         <property name="_APPDOMAIN" value="test-domain-" />
      </properties>
      <test-suite testcasecount="19" id="0-1022" runstate="Runnable" fullname="PassingCars" name="PassingCars" type="TestSuite">
         <test-suite testcasecount="19" id="0-1023" runstate="Runnable" fullname="PassingCars.Tests" name="Tests" type="TestSuite">
            <test-suite testcasecount="19" id="0-1000" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests" name="SolutionWithTestsTests" type="TestFixture" classname="PassingCars.Tests.SolutionWithTestsTests">
               <test-case id="0-1003" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_MaximumInputSizeAllOnes_0" name="solution_MaximumInputSizeAllOnes_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="2114298907" methodname="solution_MaximumInputSizeAllOnes_0" />
               <test-case id="0-1002" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_MaximumInputSizeAllZeros_0" name="solution_MaximumInputSizeAllZeros_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1942294202" methodname="solution_MaximumInputSizeAllZeros_0" />
               <test-case id="0-1004" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_MaximumOutput_MaximumOutput" name="solution_MaximumOutput_MaximumOutput" classname="PassingCars.Tests.SolutionWithTestsTests" seed="261906534" methodname="solution_MaximumOutput_MaximumOutput" />
               <test-suite testcasecount="14" id="0-1020" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.Solution_TestCases_ExpectedResults" name="Solution_TestCases_ExpectedResults" type="ParameterizedMethod" classname="PassingCars.Tests.SolutionWithTestsTests">
                  <test-case id="0-1006" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_oneElementInput0_0" name="solution_oneElementInput0_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1028238999" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1007" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_oneElementInput1_0" name="solution_oneElementInput1_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="255876234" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1008" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_manyZeros_0" name="solution_manyZeros_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="751696215" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1009" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_manyOnes_0" name="solution_manyOnes_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="459104057" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1010" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_OnePairButNotPassing_0" name="solution_OnePairButNotPassing_0" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1170663988" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1011" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_OnePairPassing_1" name="solution_OnePairPassing_1" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1194103213" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1012" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_TwoPairsPassing_3" name="solution_TwoPairsPassing_3" classname="PassingCars.Tests.SolutionWithTestsTests" seed="899212215" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1013" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_CodilityExample_5" name="solution_CodilityExample_5" classname="PassingCars.Tests.SolutionWithTestsTests" seed="2134780635" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1014" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_CodilityExampleWithAdditionalZero_5" name="solution_CodilityExampleWithAdditionalZero_5" classname="PassingCars.Tests.SolutionWithTestsTests" seed="678147389" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1015" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_CodilityExampleWithAdditional1_7" name="solution_CodilityExampleWithAdditional1_7" classname="PassingCars.Tests.SolutionWithTestsTests" seed="422784328" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1016" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_onePairInSequenceStartingFrom1_1" name="solution_onePairInSequenceStartingFrom1_1" classname="PassingCars.Tests.SolutionWithTestsTests" seed="2132525949" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1017" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_twoPairsInSequenceStartingFrom1_2" name="solution_twoPairsInSequenceStartingFrom1_2" classname="PassingCars.Tests.SolutionWithTestsTests" seed="157610855" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1018" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_sixCarsAllMakePair_9" name="solution_sixCarsAllMakePair_9" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1682898385" methodname="Solution_TestCases_ExpectedResults" />
                  <test-case id="0-1019" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_sevenCarsAllMakePair_12" name="solution_sevenCarsAllMakePair_12" classname="PassingCars.Tests.SolutionWithTestsTests" seed="773222956" methodname="Solution_TestCases_ExpectedResults" />
               </test-suite>
               <test-case id="0-1001" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_TooBigInput_ArgumentException" name="solution_TooBigInput_ArgumentException" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1040221728" methodname="solution_TooBigInput_ArgumentException" />
               <test-case id="0-1005" runstate="Runnable" fullname="PassingCars.Tests.SolutionWithTestsTests.solution_TooBigNumberOfPairs_minus1" name="solution_TooBigNumberOfPairs_minus1" classname="PassingCars.Tests.SolutionWithTestsTests" seed="1871720427" methodname="solution_TooBigNumberOfPairs_minus1" />
            </test-suite>
         </test-suite>
      </test-suite>
   </test-suite>
</test-run>

            ITest result = new TestSuite("dummy suite");
            result.AddToXml(TNode.FromXml(loaded.OuterXml), true);


            return result;
            //                }
        }

        public void UnloadProject()
        {
        }
    }
}