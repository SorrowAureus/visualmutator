namespace VisualMutator.Model.Tests.Services
{
    #region

    using Strilanc.Value;

    #endregion

    public interface ITestsService
    {
        string FrameWorkName { get; }

        May<TestsLoadContext> LoadTests(string assemblyPath);

        void Cancel();

        ITestsRunContext CreateRunContext(TestsLoadContext loadContext, string mutatedPath);
    }
}