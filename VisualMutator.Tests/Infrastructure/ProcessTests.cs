namespace VisualMutator.Tests.Infrastructure
{
    #region

    using System;
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;
    using VisualMutator.Infrastructure;

    #endregion

    [TestFixture]
    public class ProcessTests
    {
        [Test, Ignore("What it does?")]
        public void Test1()
        {
            var processes = new Processes();
            var p = new ProcessStartInfo("mspaint.exe");
            // p.
            var s = new CancellationTokenSource();
            processes.RunAsync(p, s).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    Console.WriteLine("cancelled");
                }
            });

            Assert.Inconclusive("Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe( a => s.Cancel());");

            //Thread.Sleep(10000);
        }
    }
}