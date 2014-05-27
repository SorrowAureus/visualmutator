﻿using System.Threading.Tasks;

namespace VisualMutator.Model.StoringMutants
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.Ast;
    using Mutations.Types;

    public class DisabledWhiteCache : IWhiteCache
    {
        private readonly IProjectClonesManager _fileManager;
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ProjectFilesClone _assemblies;
        private List<string> _paths;

        public DisabledWhiteCache(IProjectClonesManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void Initialize()
        {
            _assemblies = _fileManager.CreateClone("WhiteCache-");
            _paths = _assemblies.Assemblies.Select(_ => _.Path).ToList();
        }


        public CciModuleSource GetWhiteModules()
        {
            return CreateSource(_paths);

        }

        public void Reinitialize(List<string> assembliesPaths)
        {
            
        }

        public async Task<CciModuleSource> GetWhiteModulesAsync()
        {
            return await Task.Run(() => CreateSource(_paths));
        }

        public void Dispose()
        {
            _assemblies.Dispose();
        }

        public bool Paused { get; set; }

        public CciModuleSource CreateSource(IList<string> assembliesPaths)
        {
            var moduleSource = new CciModuleSource();
            foreach (var assembliesPath in assembliesPaths)
            {
                moduleSource.AppendFromFile(assembliesPath);
            }
            return moduleSource;
        }
    }
}