namespace VisualMutator.Model.StoringMutants
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface IModuleSource
    {
        List<IModuleInfo> Modules
        {
            get;
        }
    }

    internal class SimpleModuleSource : IModuleSource
    {
        public SimpleModuleSource(List<IModuleInfo> modules)
        {
            Modules = modules;
        }

        public List<IModuleInfo> Modules { get; private set; }
    }
}