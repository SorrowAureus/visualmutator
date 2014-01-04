﻿namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Infrastructure;
    using log4net;
    using Model;
    using Model.Mutations.MutantsTree;
    using UsefulTools.Core;
    using UsefulTools.DependencyInjection;
    using UsefulTools.ExtensionMethods;
    using UsefulTools.Switches;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class MainController : Controller
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MainViewModel _viewModel;
        private readonly IFactory<SessionController> _sessionControllerFactory;
        private readonly IHostEnviromentConnection _host;


        private readonly CommonServices _svc;

        private SessionController _currenSessionController;

        private List<IDisposable> _subscriptions; 


        public MainController(
            MainViewModel viewModel,
            IFactory<SessionController> sessionControllerFactory,
            IHostEnviromentConnection host,
           
            CommonServices svc)
        {
            _viewModel = viewModel;
            _sessionControllerFactory = sessionControllerFactory;
            _host = host;


            _svc = svc;


            _viewModel.CommandCreateNewMutants = new SmartCommand(RunMutationSession,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished, OperationsState.Error))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);


            _viewModel.CommandOnlyCreateMutants = new SmartCommand(OnlyCreateMutants,
                () => _viewModel.OperationsState.IsIn(OperationsState.None, OperationsState.Finished, OperationsState.Error))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);


            _viewModel.CommandPause = new SmartCommand(PauseOperations, 
                () => _viewModel.OperationsState.IsIn(OperationsState.Testing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandStop = new SmartCommand(StopOperations,
                () => _viewModel.OperationsState.IsIn(OperationsState.Mutating, OperationsState.PreCheck,
                    OperationsState.Testing, OperationsState.TestingPaused, OperationsState.Pausing))
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);

            _viewModel.CommandContinue = new SmartCommand(ResumeOperations,
                () => _viewModel.OperationsState == OperationsState.TestingPaused)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);


            _viewModel.CommandSaveResults = new SmartCommand(SaveResults, () =>
                _viewModel.OperationsState == OperationsState.Finished)
                .UpdateOnChanged(_viewModel, () => _viewModel.OperationsState);




            _viewModel.CommandTest = new SmartCommand(Test);

        }

        private void Test()
        {
            _host.Test();
        }


        private void SetState(OperationsState state)
        {
            _svc.Threading.InvokeOnGui(() =>
            {
                if (_viewModel.OperationsState != state)
                {
                    _viewModel.OperationsState = state;
                    _viewModel.OperationsStateDescription = FunctionalExt.ValuedSwitch<OperationsState, string>(state)
                        .Case(OperationsState.None, "")
                        .Case(OperationsState.TestingPaused, "Paused")
                        .Case(OperationsState.Finished, "Finished")
                        .Case(OperationsState.PreCheck, "Pre-check...")
                        .Case(OperationsState.Mutating, "Creating mutants...")
                        .Case(OperationsState.Pausing, "Pausing...")
                        .Case(OperationsState.Stopping, "Stopping...")
                        .Case(OperationsState.SavingMutants, "Saving mutants...")
                        .Case(OperationsState.Error, "Error occurred.")
                        .GetResult();
                }
            });
        }


       
        public void Subscribe(SessionController sessionController)
        {
            _subscriptions = new List<IDisposable>
            {
                sessionController.SessionEventsObservable
                    .OfType<MinorSessionUpdateEventArgs>()
                    .Subscribe(args =>
                    {
                        SetState(args.EventType);
                        Switch.On(args.ProgressUpdateMode)
                            .Case(ProgressUpdateMode.SetValue, () =>
                            {
                                _viewModel.IsProgressIndeterminate = false;
                                _viewModel.Progress = args.PercentCompleted;
                            })
                            .Case(ProgressUpdateMode.Indeterminate, () =>
                            {
                                _viewModel.IsProgressIndeterminate = true;
                            })
                            .Case(ProgressUpdateMode.PreserveValue, ()=>
                            {
                               
                            });
                        
                    }),

                sessionController.SessionEventsObservable
                 .OfType<MutationFinishedEventArgs>()
                 .Subscribe(args => _viewModel.Operators.ReplaceRange(args.MutantsGroupedByOperators)),

                sessionController.SessionEventsObservable
                 .OfType<TestingProgressEventArgs>()
                 .Subscribe(args =>
                 {
                     _svc.Threading.InvokeOnGui(()=>
                     {
                         _viewModel.OperationsState = OperationsState.Testing;
                         _viewModel.OperationsStateDescription = "Running tests... ({0}/{1})"
                             .Formatted(args.NumberOfAllMutantsTested + 1,
                                 args.NumberOfAllMutants);

                         _viewModel.MutantsRatio = string.Format("Mutants killed: {0}/{1}", args.NumberOfMutantsKilled, args.NumberOfAllMutantsTested);
                         _viewModel.MutationScore = string.Format(@"Mutation score: {0}%", args.MutationScore.AsPercentageOf(1.0d));
                         _viewModel.Progress = args.NumberOfAllMutantsTested.AsPercentageOf(args.NumberOfAllMutants);
                     });
                 }),

               
            };

            _viewModel.RegisterPropertyChanged(vm => vm.SelectedMutationTreeItem).OfType<Mutant>()
                   .Subscribe(sessionController.LoadDetails);
        }

        public void RunMutationSession()
        {
            _log.Info("Showing mutation session window.");
            Clean();
            _currenSessionController = _sessionControllerFactory.Create();
            var mutantsCreationController = _currenSessionController.MutantsCreationFactory.Create();
             mutantsCreationController.Run();
            if (mutantsCreationController.HasResults)
            {
                MutationSessionChoices choices = mutantsCreationController.Result;
               
                _viewModel.MutantDetailsViewModel = _currenSessionController.MutantDetailsController.ViewModel;

                Subscribe(_currenSessionController);

                _log.Info("Starting mutation session...");
                _currenSessionController.RunMutationSession(choices);
            }
        }
        public void RunMutationSessionForCurrentPosition()
        {
            ClassAndMethod classAndMethod;
            if (_host.GetCurrentClassAndMethod(out classAndMethod) && classAndMethod.MethodName != null)
            {
                _log.Info("Showing mutation session window.");
                Clean();
                _currenSessionController = _sessionControllerFactory.Create();
                var mutantsCreationController = _currenSessionController.MutantsCreationFactory.Create();
                mutantsCreationController.Run2(classAndMethod);
                if (mutantsCreationController.HasResults)
                {
                    MutationSessionChoices choices = mutantsCreationController.Result;

                    _viewModel.MutantDetailsViewModel = _currenSessionController.MutantDetailsController.ViewModel;

                    Subscribe(_currenSessionController);

                    _log.Info("Starting mutation session...");
                    _currenSessionController.RunMutationSession(choices);
                }
            }
            //throw new NotImplementedException();
        }
        public void OnlyCreateMutants()
        {
            Clean();
            _currenSessionController = _sessionControllerFactory.Create();
            var onlyMutantsController = _currenSessionController.OnlyMutantsCreationFactory.Create();
            onlyMutantsController.Run();

            if (onlyMutantsController.HasResults)
            {
                MutationSessionChoices choices = onlyMutantsController.Result;

                

                _viewModel.MutantDetailsViewModel = _currenSessionController.MutantDetailsController.ViewModel;
                

                
                Subscribe(_currenSessionController);


              

                _currenSessionController.OnlyCreateMutants(choices);
            }
        }

        public void PauseOperations()
        {
            SetState(OperationsState.Pausing);
            _currenSessionController.PauseOperations();
        }
        public void ResumeOperations()
        {
           _currenSessionController.ResumeOperations();
        }

        public void StopOperations()
        {
        
            _currenSessionController.StopOperations();

        }
        public void SaveResults()
        {
            
            _currenSessionController.SaveResults();
        }

        public void Initialize()
        {
            _viewModel.IsVisible = true;
        }

        public void Deactivate()
        {
            if (_currenSessionController != null)
            {
                _currenSessionController.StopOperations();
            }
            Clean();
            _viewModel.IsVisible = false;
        }

        private void Clean()
        {
            _viewModel.Clean();

          //  _mutantDetailsController.Clean();
            if (_subscriptions!=null)
            {
                foreach (var subscription in _subscriptions)
                {
                    subscription.Dispose();
                }
            }
            _currenSessionController = null;
            SetState(OperationsState.None);
        }
       
        public MainViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

       
    }
}