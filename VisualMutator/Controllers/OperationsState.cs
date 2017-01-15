namespace VisualMutator.Controllers
{
    #region

    using System;

    #endregion

    [Flags]
    public enum OperationsState
    {
        None = 0,
        Mutating = 1,
        Testing = 2,
        TestingPaused = 3,
        Pausing = 4,
        Stopping = 5,
        Finished = 6,
        PreCheck = 7,
        Error = 8,
        MutationFinished = 9,
    }
}