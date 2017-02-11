namespace VisualMutator.Model.Mutations
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Model.StoringMutants;
    using MutantsTree;

    public class MutantMaterializer
    {
        private readonly IProjectClonesManager _clonesManager;
        private readonly OriginalCodebase _originalCodebase;
        private readonly IMutantsCache _mutantsCache;

        public MutantMaterializer(IProjectClonesManager clonesManager,
            OriginalCodebase originalCodebase,
             IMutantsCache mutantsCache)
        {
            _clonesManager = clonesManager;
            _originalCodebase = originalCodebase;
            _mutantsCache = mutantsCache;
        }

        public async Task<StoredMutantInfo> StoreMutant(Mutant mutant)
        {
            mutant.State = MutantResultState.Creating;

            var mutationResult = await _mutantsCache.GetMutatedModulesAsync(mutant);

            mutant.State = MutantResultState.Writing;

            var clone = await _clonesManager.CreateCloneAsync("InitTestEnvironment");
            var info = new StoredMutantInfo(clone);

            if (mutationResult.MutatedModules != null)
            {
                //AKB
                /*foreach (ICciModuleSource mutatedModule in mutationResult.MutatedModules)
                {*/
                var singleMutated = mutationResult.MutatedModules.Modules.SingleOrDefault();
                if (singleMutated != null)
                {
                    string file = Path.Combine(info.Directory, Path.GetFileName(singleMutated.Module.Location));
                    //
                    //                    var memory = mutationResult.MutatedModules.WriteToStream(singleMutated);
                    //                    // _mutantsCache.Release(mutationResult);
                    //
                    //                    using (FileStream peStream = File.Create(file))
                    //                    {
                    //                        await memory.CopyToAsync(peStream);
                    //                    }
                    using (FileStream peStream = File.Create(file))
                    {
                        mutationResult.MutatedModules.WriteToStream(singleMutated, peStream, file);

                        //  await memory.CopyToAsync(peStream);
                    }

                    info.AssembliesPaths.Add(file);
                }

                var otherModules = _originalCodebase.Modules
                    .Where(_ => singleMutated == null || _.Module.Name != singleMutated.Name);

                foreach (var otherModule in otherModules)
                {
                    string file = Path.Combine(info.Directory, Path.GetFileName(otherModule.Module.Module.Location));
                    info.AssembliesPaths.Add(file);
                }
                //}
            }
            else
            {
                foreach (var cciModuleSource in mutationResult.Old)
                {
                    var module = cciModuleSource.Modules.Single();
                    string file = Path.Combine(info.Directory, Path.GetFileName(module.Module.Location));

                    // _mutantsCache.Release(mutationResult);

                    using (FileStream peStream = File.Create(file))
                    {
                        cciModuleSource.WriteToStream(module, peStream, file);
                        //  await memory.CopyToAsync(peStream);
                    }

                    info.AssembliesPaths.Add(file);
                }
            }

            return info;
        }
    }
}