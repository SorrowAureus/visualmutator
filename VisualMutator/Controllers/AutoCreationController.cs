﻿namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Infrastructure;
    using log4net;
    using Microsoft.Cci;
    using Model;
    using Model.Exceptions;
    using Model.Mutations.MutantsTree;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.StoringMutants;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.Paths;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class AutoCreationController 
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private readonly SessionConfiguration _sessionConfiguration;
        private readonly OptionsModel _options;
        private readonly IFactory<SessionCreator> _sessionCreatorFactory;
        private readonly IDispatcherExecute _execute;

        private readonly CommonServices _svc;
        private readonly CreationViewModel _viewModel;
        private readonly ITypesManager _typesManager;

        public MutationSessionChoices Result { get; protected set; }


        public AutoCreationController(
            CreationViewModel viewModel,
            ITypesManager typesManager,
            SessionConfiguration sessionConfiguration,
            OptionsModel options,
            IFactory<SessionCreator> sessionCreatorFactory,
            IDispatcherExecute execute,
            CommonServices svc)
        {
            _viewModel = viewModel;
            _typesManager = typesManager;

            _sessionConfiguration = sessionConfiguration;
            _options = options;
            _sessionCreatorFactory = sessionCreatorFactory;
            _execute = execute;
            _svc = svc;

            
            _viewModel.CommandCreateMutants = new SmartCommand(AcceptChoices,
               () => _viewModel.TypesTreeMutate.Assemblies != null && _viewModel.TypesTreeMutate.Assemblies.Count != 0
                     && _viewModel.TypesTreeToTest.TestAssemblies != null && _viewModel.TypesTreeToTest.TestAssemblies.Count != 0
                     && _viewModel.MutationsTree.MutationPackages.Count != 0)
               .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
               .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.TestAssemblies)
               .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);
        }

       

        public async Task<MutationSessionChoices> Run(MethodIdentifier singleMethodToMutate = null, bool auto = false)
        {
            SessionCreationWindowShowTime = DateTime.Now;

            if(_sessionConfiguration.AssemblyLoadProblem)
            {
                _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _viewModel.View);
            }

            bool constrainedMutation = false;
            ICodePartsMatcher matcher;
            if (singleMethodToMutate != null)
            {
                matcher = new CciMethodMatcher(singleMethodToMutate);
                constrainedMutation = true;
            }
            else
            {
                matcher = new AllMatcher();
            }


            SessionCreator sessionCreator = _sessionCreatorFactory.Create();

            Task<IModuleSource> assembliesTask = _sessionConfiguration.LoadAssemblies();

            Task<List<MethodIdentifier>> coveringTask = sessionCreator.FindCoveringTests(assembliesTask, matcher);

            Task<object> testsTask = _sessionConfiguration.LoadTests();

            var t1 = sessionCreator.GetOperators();

            var t2 = sessionCreator.BuildAssemblyTree(assembliesTask, constrainedMutation, matcher);

            var t3 = sessionCreator.BuildTestTree(coveringTask, testsTask, constrainedMutation);

            t1.ContinueWith(task =>
            {
                _viewModel.MutationsTree.MutationPackages
                    = new ReadOnlyCollection<PackageNode>(task.Result.Packages);
            }, TaskContinuationOptions.NotOnFaulted);

            t2.ContinueWith(task =>
            {
                _viewModel.TypesTreeMutate.Assemblies = new ReadOnlyCollection<AssemblyNode>(task.Result);
            }, TaskContinuationOptions.NotOnFaulted);

            t3.ContinueWith(task =>
            {
                _viewModel.TypesTreeToTest.TestAssemblies
                                = new ReadOnlyCollection<TestNodeAssembly>(task.Result);
            }, TaskContinuationOptions.NotOnFaulted);

            try
            {
                var tt = Task.WhenAll(t1, t2, t3).ContinueWith(t =>
                {
                    if (t1.Exception != null)
                    {
                        ShowError(t1.Exception);
                        _viewModel.Close();
                    }
                    else
                    {
                        if(auto)
                        {
                            AcceptChoices();
                        }
                    }
                }, _execute.GuiScheduler);

                _viewModel.ShowDialog();
                await tt;
                return Result;
              
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }

        protected void AcceptChoices()
        {
            if (_viewModel.TypesTreeToTest.TestAssemblies.All(a => a.IsIncluded == false))
            {
                _svc.Logging.ShowError(UserMessages.ErrorNoTestsToRun(), _viewModel.View);
                return;
            }
            Result = new MutationSessionChoices
            {
                SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                    .Where(oper => (bool)oper.IsIncluded).Select(n => n.Operator).ToList(),
                Filter = _typesManager.CreateFilterBasedOnSelection(_viewModel.TypesTreeMutate.Assemblies),
                TestAssemblies = _viewModel.TypesTreeToTest.TestAssemblies,
                MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                MutantsTestingOptions = _viewModel.MutantsTesting.Options,
                MainOptions = _options
            };

            _viewModel.Close();
        }

        public DateTime SessionCreationWindowShowTime { get; set; }

        private void CheckError(Task result)
        {
            if (result.Exception != null)
            {
                ShowErrorAndExit(result.Exception);
            }
        }

        private void ShowErrorAndExit(AggregateException exception)
        {
            _log.Error(exception);
            _svc.Threading.PostOnGui(() =>
            {
                ShowError(exception);
                _viewModel.Close();
            });
        }

        private void ShowError(Exception exc)
        {
            var aggregate = exc as AggregateException;
            Exception innerException = aggregate == null ? exc : aggregate.Flatten().InnerException;
            if (innerException is TestWasSelectedToMutateException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorBadMethodSelected(), _viewModel.View);
            }
            else if (innerException is StrongNameSignedAssemblyException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorStrongNameSignedAssembly(), _viewModel.View);
            }
            else if (innerException is TestsLoadingException)
            {
                _svc.Logging.ShowError(UserMessages.ErrorTestsLoading(), _viewModel.View);
            }
            else
            {
                _svc.Logging.ShowError(exc, _viewModel.View);
            }
        }



      
    }
}