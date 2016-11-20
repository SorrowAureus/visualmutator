using System.Linq;
using NUnit.Framework;

namespace VisualMutator.Model.Mutations.Operators.Tests
{
    [TestFixture()]
    public class MEFOperatorLoaderTests
    {
        [Test()]
        public void ReloadOperatorsTest()
        {
            var subject = new MEFOperatorLoader();

            var result = subject.ReloadOperators();

            var resultEnumerator = result.GetEnumerator();

            resultEnumerator.MoveNext();

            Assert.AreEqual(resultEnumerator.Current.Description, "Object oriented Operators.");

            var OOOperators = resultEnumerator.Current.Operators.Select(p => p.Info.Id);

            CollectionAssert.Contains(OOOperators, "DEH");
            CollectionAssert.Contains(OOOperators, "DMC");
            CollectionAssert.Contains(OOOperators, "EMM");
            CollectionAssert.Contains(OOOperators, "EAM");
            CollectionAssert.Contains(OOOperators, "EHC");
            CollectionAssert.Contains(OOOperators, "EHR");
            CollectionAssert.Contains(OOOperators, "EXS");
            CollectionAssert.Contains(OOOperators, "ISD");
            CollectionAssert.Contains(OOOperators, "JID");
            CollectionAssert.Contains(OOOperators, "JTD");
            CollectionAssert.Contains(OOOperators, "PRV");
            CollectionAssert.Contains(OOOperators, "MCI");

            resultEnumerator.MoveNext();

            Assert.AreEqual(resultEnumerator.Current.Description, "Standard imperative operators.");

            var StdOperators = resultEnumerator.Current.Operators.Select(p => p.Info.Id);

            CollectionAssert.Contains(StdOperators, "AOR");
            CollectionAssert.Contains(StdOperators, "SOR");
            CollectionAssert.Contains(StdOperators, "LCR");
            CollectionAssert.Contains(StdOperators, "LOR");
            CollectionAssert.Contains(StdOperators, "ROR");
            CollectionAssert.Contains(StdOperators, "OODL");
            CollectionAssert.Contains(StdOperators, "SSDL");
        }
    }
}