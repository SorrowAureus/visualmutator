using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SomeAnotherTestProjectNamespace
{
    [TestFixture]
    public class UnitTest2
    {
        [TestCase(3)]
        [TestCase(4)]
        public void TestMethod2(int a)
        {
        }
    }
}