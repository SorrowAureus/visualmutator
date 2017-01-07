namespace VisualMutator.Infrastructure
{
    #region

    using System;
    using System.Collections.Generic;
    using Model.CoverageFinder;
    using UsefulTools.Paths;

    #endregion

    public enum EventType
    {
        HostOpened,
        HostClosed,
        BuildBegin,
        BuildDone
    }

    public interface IHostEnviromentConnection
    {
        void Initialize();

        IObservable<EventType> Events { get; }

        IEnumerable<FilePathAbsolute> GetProjectAssemblyPaths();

        string GetTempPath();

        void Test();

        bool GetCurrentClassAndMethod(out MethodIdentifier methodIdentifier);

        void Build();
    }
}