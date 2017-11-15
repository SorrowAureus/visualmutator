﻿namespace VisualMutator.Console {
  #region

  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Threading.Tasks;
  using log4net;
  using Model;
  using Model.CoverageFinder;
  using Ninject.Modules;
  using VisualMutator.Infrastructure;
  using VisualMutator.Infrastructure.NinjectModules;

  #endregion

  public class ConsoleBootstrapper {
    private readonly EnvironmentConnection _connection;
    private readonly CommandLineParser _parser;
    private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly Bootstrapper _boot;

    public ConsoleBootstrapper(EnvironmentConnection connection, CommandLineParser parser) {
      _connection = connection;
      _parser = parser;
      _boot = new Bootstrapper(new List<INinjectModule>() {
                new VisualMutatorModule(),
                new ConsoleInfrastructureModule(),
                new FakeViewsModule(),
                new ConsoleNinjectModule(connection)});
    }

    public object Shell => _boot.Shell;

    public async Task Initialize() {
      try {
        _boot.Initialize();
        OptionsModel optionsModel = _boot.AppController.OptionsManager.ReadOptions();
        optionsModel.WhiteCacheThreadsCount = _parser.SourceThreads;//1;
        optionsModel.ProcessingThreadsCount = _parser.MutationThreads;
        optionsModel.MutantsCacheEnabled = false;
        optionsModel.ParsedParams = _parser.OtherParams;
        Console.WriteLine(_parser.OtherParams.ToString());
        _boot.AppController.OptionsManager.WriteOptions(optionsModel);

        MethodIdentifier methodIdentifier;
        _connection.GetCurrentClassAndMethod(out methodIdentifier);

        var tcs = new TaskCompletionSource<object>();

        _boot.AppController.MainController.SessionFinishedEvents.Subscribe(_ => {
          tcs.SetResult(new object());
        });

        await _boot.AppController.MainController.RunMutationSession(methodIdentifier, _parser.TestAssembliesList, true);

        await tcs.Task;

        await _boot.AppController.MainController.SaveResultsAuto(_parser.ResultsXml);
        //                for (int i = 0; i < 1000; i++)
        //                {
        //                    _boot.AppController.MainController.RunMutationSessionAuto2(methodIdentifier);
        //                }

        _connection.End();
      }
      catch (Exception e) {
        _log.Error(e);
      }
    }
  }
}