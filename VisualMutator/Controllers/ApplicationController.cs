﻿namespace PiotrTrzpil.VisualMutator_VSPackage.Controllers
{
    #region Usings

    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;

    using CommonUtilityInfrastructure.WpfUtils;

    using PiotrTrzpil.VisualMutator_VSPackage.Infrastructure.WpfUtils.Messages;
    using PiotrTrzpil.VisualMutator_VSPackage.Model;
    using PiotrTrzpil.VisualMutator_VSPackage.ViewModels;

    using VisualMutator.Infrastructure;

    using log4net;

    #endregion

    public class ApplicationController : IHandler<SwitchToUnitTestsTabEventArgs>
    {
        private readonly ILMutationsController _ilMutationsController;

        private readonly MainWindowViewModel _viewModel;

        private readonly UnitTestsController _unitTestsController;

        private readonly IVisualStudioConnection _visualStudio;

        private readonly IMessageService _messageService;

        private readonly IEventService _eventService;

        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ApplicationController(
            MainWindowViewModel viewModel,
            ILMutationsController ilMutationsController,
            UnitTestsController unitTestsController,
            IVisualStudioConnection visualStudio,
            IMessageService messageService,
            IEventService eventService)
        {
            _viewModel = viewModel;
            _ilMutationsController = ilMutationsController;
            _unitTestsController = unitTestsController;
            _visualStudio = visualStudio;
            _messageService = messageService;
            _eventService = eventService;

            _viewModel.ILMutationsView = _ilMutationsController.ILMutationsVm.View;
            _viewModel.UnitTestsView = _unitTestsController.UnitTestsVm.View;

            HookGlobalExceptionHandlers();

            _eventService.Subscribe(this);
         
        }

        public object Shell
        {
            get
            {
                return _viewModel.View;
            }
        }

        public void Initialize()
        {
            _visualStudio.Initialize();

            _visualStudio.SolutionOpened += ActivateOnSolutionOpened;
            _visualStudio.SolutionAfterClosing += DeactivateOnSolutionClosed;

            _viewModel.SelectedTab = 0;

        }
        public void HookGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _messageService.ShowError(e.Exception,_log );
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;


            _messageService.ShowError(exception, _log);

        }


        private void ActivateOnSolutionOpened()
        {
            _ilMutationsController.Initialize();
            _unitTestsController.Initialize();
        }
        private void DeactivateOnSolutionClosed()
        {
            _ilMutationsController.Deactivate();
            _unitTestsController.Deactivate();
        }

        public void Handle(SwitchToUnitTestsTabEventArgs message)
        {
            _viewModel.SelectedTab = 1;
        }
    }
}