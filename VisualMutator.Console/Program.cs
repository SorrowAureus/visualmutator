using System;
using CommandLine;

namespace VisualMutator.Console {
  using System.Threading.Tasks;
  using Console = System.Console;
    //--sourceAssemblies D:\ovs\Codility\MaxCountersTests\bin\Debug\MaxCounters.dll --testAssemblies D:\ovs\Codility\MaxCountersTests\bin\Debug\MaxCountersTests.dll --resultsXml d:\pavzaj.xml
  internal class Program {
    private static void Main(string[] args) {
      Console.WriteLine("Started VisualMutator.Console with params: " + args.MakeString());

      if (args.Length >= 5)
      {
        var parser = new CommandLineParser();
        if (Parser.Default.ParseArguments(args, parser))
        {
          var connection = new EnvironmentConnection(parser);
          var boot = new ConsoleBootstrapper(connection, parser);
          boot.Initialize().GetAwaiter().GetResult();
        }
        else
        {
          //
          // var str = options.LastParserState.Errors.Select(a=>a.ToString()).Aggregate((a, b) => a.ToString() + "n" + b.ToString());
          Console.WriteLine("Invalid params string in options.: " + args);
        }
      }
      else {
        Console.WriteLine("Too few parameters.");
      }
    }
  }
}