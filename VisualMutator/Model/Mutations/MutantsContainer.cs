﻿namespace VisualMutator.Model.Mutations
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Exceptions;
    using Extensibility;
    using log4net;
    using Microsoft.Cci;
    using Microsoft.Cci.MutableCodeModel;
    using MutantsTree;
    using Operators;
    using Types;
    using UsefulTools.CheckboxedTree;
    using UsefulTools.Core;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Paths;
    using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;

    #endregion

    public interface IMutantsContainer
    {
        void Initialize(MutantsCreationOptions options, MutationFilter filter);

       
        Mutant CreateEquivalentMutant(out ExecutedOperator executedOperator);

        void SaveMutantsToDisk(MutationTestingSession currentSession);

        ModulesProvider ExecuteMutation(Mutant mutant, ProgressCounter percentCompleted, ModuleSource moduleSource);

        MutantsContainer.OperatorWithTargets FindTargets(IMutationOperator oper, IList<IModule> assemblies);

        List<ExecutedOperator> InitMutantsForOperators(ICollection<IMutationOperator> operators, ModulesProvider originalModules, ProgressCounter percentCompleted);
    }

    public class MutantsContainer : IMutantsContainer
    {
        private readonly IModuleSource _cci;
        private readonly IOperatorUtils _operatorUtils;

        private bool _debugConfig ;

        public bool DebugConfig
        {
            get { return _debugConfig; }
            set { _debugConfig = value; }
        }

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MutantsCreationOptions _options;
        private MutationFilter _filter;

        public MutantsContainer(IModuleSource cci, 
            IOperatorUtils operatorUtils
        
            )
        {
            _cci = cci;
            _operatorUtils = operatorUtils;

        }

        public void Initialize(MutantsCreationOptions options, MutationFilter filter)
        {
            _options = options;
            _filter = filter;
        }

        public Mutant CreateEquivalentMutant(out ExecutedOperator executedOperator)
        {
            var op = new IdentityOperator();
          
            executedOperator = new ExecutedOperator("E", "Equivalency operator", op);
            var group = new MutantGroup("Testing original program", executedOperator);
            var target = new MutationTarget(new MutationVariant())
                         {
                             Name = "Original program",
                         };
            var mutant = new Mutant("0", group, target, new List<MutationTarget>());
           
            group.Children.Add(mutant);
            executedOperator.Children.Add(group);
            executedOperator.UpdateDisplayedText();
            group.UpdateDisplayedText();
            mutant.UpdateDisplayedText();
            return mutant;
        }

        public void SaveMutantsToDisk(MutationTestingSession currentSession)
        {
            
        }


        public List<ExecutedOperator> InitMutantsForOperators(ICollection<IMutationOperator> operators,
            ModulesProvider originalModules, ProgressCounter percentCompleted)
        {
            var mutantsGroupedByOperators = new List<ExecutedOperator>();
            var root = new MutationRootNode();

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;


            percentCompleted.Initialize(operators.Count);
            var subProgress = percentCompleted.CreateSubprogress();

            var sw = new Stopwatch();


            var operatorsWithTargets = new List<ExecutedOperator>();
            foreach (var oper in operators)
            {
                var executedOperator = new ExecutedOperator(oper.Info.Id, oper.Info.Name, oper);

                sw.Restart();

                OperatorWithTargets operatorResult = FindTargets(oper, originalModules.Assemblies);

                executedOperator.FindTargetsTimeMiliseconds = sw.ElapsedMilliseconds;

                IEnumerable<MutationTarget> mutations = LimitMutationTargets(operatorResult.MutationTargets);
                var groupedTargets = mutations.ToLookup(target => target.GroupName);
                //subProgress.Initialize(targets.MutationTargets.Count);
                foreach (var grouping in groupedTargets.OrderBy(g => g.Key))
                {
                    var group = new MutantGroup(grouping.Key, executedOperator);
                    foreach (var mutationTarget in grouping)
                    {
                        var mutant = new Mutant(genId().ToString(), group, mutationTarget, operatorResult.CommonTargets);
                        group.Children.Add(mutant);

                    }
                    executedOperator.Children.Add(group);
                    group.UpdateDisplayedText();
                    //subProgress.Progress();
                }
                operatorsWithTargets.Add(executedOperator);
                executedOperator.UpdateDisplayedText();
                mutantsGroupedByOperators.Add(executedOperator);
                //executedOperator.Parent = root;
                //root.Children.Add(executedOperator);

                percentCompleted.Progress();
            }

           // var classes = operatorsWithTargets.


            root.State = MutantResultState.Untested;

            return mutantsGroupedByOperators;
        }

        private IList<MutationTarget> LimitMutationTargets(IEnumerable<MutationTarget> targets)
        {
           // return targets.ToList();
            var mapping = targets//.Shuffle()
              //  .SelectMany(pair => pair.Item2.Select(t => Tuple.Create(pair.Item1, t))).Shuffle()
                .Take(_options.MaxNumerOfMutantPerOperator).ToList();
            return mapping;
        }


        public OperatorWithTargets FindTargets(IMutationOperator mutOperator, IList<IModule> modules)
        {
            _log.Info("Finding targets for mutation operator: " + mutOperator.Info);

            try
            {
                var commonTargets = new List<MutationTarget>();
              
                var ded = mutOperator.CreateVisitor();
                IOperatorCodeVisitor operatorVisitor = ded;
                operatorVisitor.Host = _cci.Host;
                operatorVisitor.OperatorUtils = _operatorUtils;
                operatorVisitor.Initialize();
                var mergedTargets = new List<MutationTarget>();
                foreach (var module in modules)
                {
                    var visitor = new VisualCodeVisitor(operatorVisitor, module);

                    var traverser = new VisualCodeTraverser(_filter, visitor);
                  
                    traverser.Traverse(module);
                    visitor.PostProcess();


                  //  var assemblyNode = BuildMutantsTree(module, visitor.MutationTargets, visitor.SharedTargets);

                    mergedTargets.AddRange(visitor.MutationTargets);
                    commonTargets.AddRange(visitor.SharedTargets);
                }

                _log.Info("Found total of: " + mergedTargets.Count() + " mutation targets.");
         
                return new OperatorWithTargets
                {
                    CommonTargets = commonTargets,
                    MutationTargets = mergedTargets,
                    Operator = mutOperator,
              
                };
            }
            catch (Exception e)
            {
                if (!DebugConfig)
                {
                    throw new MutationException("Finding targets operation failed in operator: {0}.".Formatted(mutOperator.Info.Name), e);
                }
                else
                {
                    throw;
                }
            }
            

        }

        private AssemblyNode BuildMutantsTree(IModule module, List<MutationTarget> mutationTargets, List<MutationTarget> sharedTargets)
        {

            int[] id = { 1 };
            Func<int> genId = () => id[0]++;


            var assemblyNode = new AssemblyNode(module.Name.Value, module);


            System.Action<CheckedNode, List<MutationTarget>> typeNodeCreator = (parent, targets) =>
                {

                    foreach (var byTypeGrouping in targets.GroupBy(t => t.TypeName))
                    {
                        var type = new TypeNode(parent, byTypeGrouping.Key);
                        parent.Children.Add(type);

                        var groupedByMethodTargets = byTypeGrouping.GroupBy(target => target.MethodRaw.Name.Value);
                        foreach (var byMethodGrouping in groupedByMethodTargets.OrderBy(g => g.Key))
                        {
                            var md = byMethodGrouping.First().MethodRaw;
                            var method = new MethodNode(type, md.Name.Value, md, true);
                            type.Children.Add(method);

                            var groupedTargets = byMethodGrouping.GroupBy(target => target.GroupName);
                            foreach (var byGroupGrouping in groupedTargets.OrderBy(g => g.Key))
                            {
                                var group = new MutantGroup(byGroupGrouping.Key, method);
                                method.Children.Add(group);
                                foreach (var mutationTarget in byGroupGrouping)
                                {
                                    var mutant = new Mutant(genId().ToString(), group, mutationTarget, sharedTargets);
                                    group.Children.Add(mutant);

                                }
                                group.UpdateDisplayedText();
                            }
                        }
                        

                    }
                    
                };
            Func<MutationTarget, string> namespaceExtractor = target => target.NamespaceName;
            Func<MutationTarget, string> nameExtractor = target => target.TypeName;

            new NamespaceGrouper().
                GroupTypes2(assemblyNode, "", namespaceExtractor, nameExtractor, typeNodeCreator, mutationTargets);


            return assemblyNode;


        }

        public ModulesProvider ExecuteMutation(Mutant mutant,  ProgressCounter percentCompleted, 
            ModuleSource moduleSource)
        {
            try
            {
                _log.Info("Execute mutation of " + mutant.MutationTarget + " contained in " + mutant.MutationTarget.MethodRaw + " modules. " );
              //  var cci = new ModuleSource();
                var mutatedModules = new List<Assembly>();
                var oldVersions = new Dictionary<Version, Version>();
                foreach (var module in moduleSource.Modules)
                {
                    percentCompleted.Progress();
                    var visitorBack = new VisualCodeVisitorBack(mutant.MutationTarget.InList(),
                        mutant.CommonTargets, module);
                    var traverser2 = new VisualCodeTraverser(_filter, visitorBack);
                    traverser2.Traverse(module);
                    visitorBack.PostProcess();
                    var operatorCodeRewriter = mutant.ExecutedOperator.Operator.CreateRewriter();

                    var rewriter = new VisualCodeRewriter(_cci.Host, visitorBack.TargetAstObjects,
                        visitorBack.SharedAstObjects, _filter, operatorCodeRewriter);

                    operatorCodeRewriter.MutationTarget =
                        new UserMutationTarget(mutant.MutationTarget.Variant.Signature, mutant.MutationTarget.Variant.AstObjects);
                    
                    operatorCodeRewriter.NameTable = _cci.Host.NameTable;
                    operatorCodeRewriter.Host = _cci.Host;
                    operatorCodeRewriter.Module = module;
                    operatorCodeRewriter.OperatorUtils = _operatorUtils;

                    operatorCodeRewriter.Initialize();

                    Assembly rewrittenModule = (Assembly) rewriter.Rewrite(module);
                    var oldver = rewrittenModule.Version;
                    
                  //  rewrittenModule.Version = new Version(rewrittenModule.Version.Major, rewrittenModule.Version.Minor,
                  //      rewrittenModule.Version.Build, new Random().Next(int.MaxValue));
                    mutatedModules.Add(rewrittenModule);
                 //   oldVersions.Add(oldver, rewrittenModule.Version);
                }
              /*  foreach (var m in mutatedModules)
                {
                    foreach (AssemblyReference refe in m.AssemblyReferences)
                    {
                        if(oldVersions.ContainsKey(refe.Version))
                        {
                            refe.Version = oldVersions[refe.Version];
                        }
                    }
                    var s = m.AssemblyReferences.Select(a => a.AssemblyIdentity.ToString());
                    _log.Debug(s);
                }*/
                return new ModulesProvider(mutatedModules.Cast<IModule>().ToList());
            }
            catch (Exception e)
            {
                if (!DebugConfig)
                {
                    throw new MutationException("CreateMutants failed on operator: {0}.".Formatted(mutant.ExecutedOperator.Operator.Info.Name), e);
                }
                else
                {
                    throw;
                }
                
            }
        }
        public class OperatorWithTargets
        {
            public List<MutationTarget> MutationTargets
            {
                get;
                set;
            }

            public IMutationOperator Operator
            {
                get;
                set;
            }



            public List<MutationTarget> CommonTargets
            {
                get;
                set;
            }

        }


    }
}