﻿namespace VisualMutator.Model
{
    using JetBrains.Annotations;
    using Microsoft.Cci;

    public interface IModuleInfo
    {
        IAssembly Module
        {
            get; set;
        }

        string Name
        {
            get;
        }
    }

    public class ModuleInfo : IModuleInfo
    {
        public ModuleInfo(IAssembly module, string path = null)
        {
            Module = module;
        }

        public IAssembly Module
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return Module.Name.Value;
            }
        }


        public PdbReader PdbReader
        {
            get; set;
        }


        public ILocalScopeProvider LocalScopeProvider
        {
            get; set;
        }


        public CciModuleSource SubCci
        {
            get; set;
        }
    }
}