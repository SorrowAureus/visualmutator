namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;

    using Strilanc.Value;

    public interface ITestsService
    {
        string FrameWorkName { get; }

        May<IEnumerable<TestsLoadContext>> LoadTests(IEnumerable<string> assemblyPath);

        void Cancel();

        ITestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath);
    }
}