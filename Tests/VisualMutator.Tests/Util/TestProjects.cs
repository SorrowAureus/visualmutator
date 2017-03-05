using System;
using System.IO;
using System.Reflection;

namespace VisualMutator.Tests.Util
{
    public static class TestProjects
    {
        public const string DsaPath = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Testy\Dsa\x86\Dsa.dll";
        public const string DsaTestsPath = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Testy\Dsa\x86\Dsa.Test.dll";
        public const string DsaTestsPath2 = @"C:\PLIKI\Dropbox\++Inzynierka\VisualMutator\Testy\Dsa\x86\Dsa.Test2.dll";
        public const string MiscUtil = @"D:\github\Visualmutator\visualmutatorpavzaj\packages\MiscUtil.dll";
        public const string MiscUtilTests = @"C:\PLIKI\VisualMutator\testprojects\NUnit\MiscUtil\MiscUtil.UnitTests\bin\Debug\MiscUtil.UnitTests.dll";
        public static readonly string AutoMapper = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\packages\AutoMapper.5.2.0\lib\net45\AutoMapper.dll"));
        public static readonly string AutoMapperNet4 = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\packages\AutoMapper.5.2.0\lib\net45\AutoMapper.dll"));
        public static readonly string AutoMapperTests = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\packages\AutoMapper.5.2.0\lib\net45\AutoMapper.dll"));

        public static readonly string NUnitConsoleDirPath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\packages\NUnit.ConsoleRunner.3.6.0\tools"));

        public static readonly string NUnitConsolePath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), @"..\..\..\..\packages\NUnit.ConsoleRunner.3.6.0\tools\nunit3-console.exe"));

        public const string XUnitConsoleDirPath =
                   @"C:\PLIKI\VisualMutator\xunitconsole";

        public const string XUnitConsolePath =
                  @"C:\PLIKI\VisualMutator\xunitconsole\xunit.console.exe";
    }
}