﻿namespace VisualMutator.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.Threading;
    using CommonUtilityInfrastructure.WpfUtils;
    using Model;
    using Model.Tests;
    using Model.Tests.TestsTree;
    using VisualMutator.Infrastructure;
    using VisualMutator.Model.Mutations;
    using VisualMutator.Model.Mutations.Operators;
    using VisualMutator.Model.Mutations.Types;
    using VisualMutator.ViewModels;
    using VisualMutator.Views;

    using log4net;

    #endregion

    public abstract class CreationController<TViewModel, TView> : Controller
        where TViewModel : CreationViewModel<TView> where TView : class, IWindow
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



        protected readonly IOperatorsManager _operatorsManager;
        private readonly IHostEnviromentConnection _hostEnviroment;
        protected readonly ITestsContainer _testsContainer;

        protected readonly CommonServices _svc;

        protected readonly ITypesManager _typesManager;

        protected readonly TViewModel _viewModel;

     
        public MutationSessionChoices Result { get; protected set; }

        protected CreationController(
            TViewModel viewModel,
            ITypesManager typesManager,
            IOperatorsManager operatorsManager,
             IHostEnviromentConnection hostEnviroment,
            ITestsContainer testsContainer,
            CommonServices svc)
        {
            _viewModel = viewModel;

            _typesManager = typesManager;
            _operatorsManager = operatorsManager;
            _hostEnviroment = hostEnviroment;
            _testsContainer = testsContainer;
            _svc = svc;


            _viewModel.CommandCreateMutants = new BasicCommand(AcceptChoices,
                () => _viewModel.TypesTreeMutate.Assemblies.Count != 0
                    && _viewModel.TypesTreeToTest.Namespaces.Count != 0
                    && _viewModel.MutationsTree.MutationPackages.Count != 0)
                    .UpdateOnChanged(_viewModel.TypesTreeMutate, _ => _.Assemblies)
                    .UpdateOnChanged(_viewModel.TypesTreeToTest, _ => _.Namespaces)
                    .UpdateOnChanged(_viewModel.MutationsTree, _ => _.MutationPackages);

   
           
        }

        public async void Run()
        {
            _svc.Threading.PostOnGui(async () =>
                {


                  //  var assemblies = await Task.Run(() => _typesManager.GetTypesFromAssemblies());


                 //   _viewModel.TypesTreeMutate.Assemblies = assemblies.ToReadonly();



                    var taskGetAssemblies = Task.Run<object>(() => _typesManager.GetTypesFromAssemblies());
                    var taskLoadTests = Task.Run<object>(() => _testsContainer.LoadTests(
                                _hostEnviroment.GetProjectAssemblyPaths().Select(p => (string) p).ToList()));
                    var loadOperators = Task.Run<object>(() => _operatorsManager.LoadOperators());


                    var tAll = await Task.WhenAll(taskGetAssemblies, taskLoadTests, loadOperators);

                  //  t.
                    if (_typesManager.IsAssemblyLoadError)
                    {

                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _log, _viewModel.View);
                    }

                    _viewModel.TypesTreeMutate.Assemblies = tAll[0].CastTo<IList<AssemblyNode>>().ToReadonly();
                    _viewModel.TypesTreeToTest.Namespaces = tAll[1].CastTo<IEnumerable<TestNodeNamespace>>().ToReadonly();
                

                   // var packages = await Task.Run(() => _operatorsManager.LoadOperators());

                    _viewModel.MutationsTree.MutationPackages = tAll[2].CastTo<IList<PackageNode>>().ToReadonly();


                });
            /*
         //   _svc.Threading.ScheduleAsync(()=> _operatorsManager.LoadOperators(),
         //       packages => _viewModel.MutationsTree.MutationPackages = new ReadOnlyCollection<PackageNode>(packages));

            _svc.Threading.ScheduleAsync(() => _typesManager.GetTypesFromAssemblies(),
                assemblies =>
                {
                    _viewModel.TypesTreeMutate.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);
                  //  _viewModel.TypesTreeToTest.Assemblies =  new ReadOnlyCollection<AssemblyNode>(assemblies);


                    var testNodeNamespaces = _testsContainer.LoadTests(_hostEnviroment.GetProjectAssemblyPaths().Select(p => (string)p).ToList());
                    
                    if (_typesManager.IsAssemblyLoadError)
                    {
                        
                        _svc.Logging.ShowWarning(UserMessages.WarningAssemblyNotLoaded(), _log, _viewModel.View);
                    }
                });
            */
            _viewModel.ShowDialog();
    
        }

        protected abstract void AcceptChoices();


        public bool HasResults
        {
            get
            {
                return Result != null;
            }
        }

    }
}