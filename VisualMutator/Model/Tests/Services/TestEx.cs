namespace VisualMutator.Model.Tests.Services
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework.Interfaces;

    #endregion

    public static class TestEx
    {
        public static IEnumerable<ITest> TestsEx(this ITest test)
        {
            return (test.Tests ?? new List<ITest>()).Cast<ITest>();
        }
    }
}