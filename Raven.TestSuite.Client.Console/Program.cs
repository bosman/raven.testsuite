using Raven.Client;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using Raven.TestSuite.Common;
    using Raven.TestSuite.TestRunner;

    public class Program
    {
        private static RavenTestRunner runner;

        private static IDocumentStore docStore;

        public static void Main(string[] args)
        {
            docStore = new Raven.Client.Embedded.EmbeddableDocumentStore
            {
                DataDirectory = "Data",
                UseEmbeddedHttpServer = true
            }.Initialize();

            SubscribeToApplicationExit();

            runner = new RavenTestRunner();
            var progressIndicator = new Progress<ProgressReport>(progressReport => Console.WriteLine(progressReport.Message));
            var cancellationtokenSource = new CancellationTokenSource();
            var token = cancellationtokenSource.Token;
            var versionsList = new List<string> { "C:\\RavenDB-Build-2750" };
            //var versionsList = new List<string> { "C:\\RavenDB-Unstable-Build-2804" };
            //var versionsList = new List<string> { "C:\\RavenDB-Build-2750", "C:\\RavenDB-Unstable-Build-2804" };
            var testRunSetup = new TestRunSetup {RavenVersionPath = versionsList};
            var task = runner.RunAllTests(progressIndicator, token, testRunSetup);
            task.ContinueWith(continuation =>
                {
                    if (continuation.IsCanceled)
                    {
                        Console.WriteLine("Tests cancelled");
                    }
                    else if (continuation.IsCompleted)
                    {
                        StoreAndDisplayResults(continuation.Result);
                    }
                });

            var shouldQuit = false;
            while (!shouldQuit)
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                        case ConsoleKey.T:
                            Console.WriteLine("Test");
                        break;
                        case ConsoleKey.C:
                            cancellationtokenSource.Cancel();
                        break;
                        case ConsoleKey.Escape:
                            shouldQuit = true;
                        break;
                }
            }
            Console.WriteLine("Exiting. Please wait...");
        }

        private static void SubscribeToApplicationExit()
        {
#if DEBUG
            var processes = Process.GetProcessesByName("Raven.Server");
            foreach (var process in processes)
                process.Kill();
#endif

            if (Process.GetProcessesByName("Raven.Server").Length > 0) 
                throw new InvalidOperationException("Cannot start tests when there are Raven.Server's running.");

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            ShutDown();
        }

        private static void OnProcessExit(object sender, EventArgs eventArgs)
        {
            ShutDown();
        }

        private static void ShutDown()
        {
            if (runner == null)
                return;

            runner.Dispose();
            runner = null;
        }

        private static void StoreAndDisplayResults(IEnumerable<TestRun> testRuns)
        {
            Console.WriteLine("=====Test Run Results=====");
            foreach (var testRun in testRuns)
            {
                var ravenTestRun = RavenTestRun.FromTestRun(testRun);
                using (var session = docStore.OpenSession())
                {
                    Console.WriteLine(
                        string.Format("Raven version: {0} (Started: {1}, Stopped: {2})",
                                      testRun.RavenVersion, testRun.StartedAt.ToString(),
                                      testRun.StoppedAt.ToString()));

                    foreach (var testResult in testRun.TestResults)
                    {
                        var ravenTestResult = RavenTestResult.FromTestResult(testResult);
                        session.Store(ravenTestResult);
                        ravenTestRun.RavenTestResultIds.Add(ravenTestResult.Id);

                        Console.WriteLine("Test name: " + testResult.TestName);
                        if (testResult.IsSuccess)
                        {
                            Console.WriteLine("Success");
                        }
                        else
                        {
                            Console.WriteLine("Failure: " + testResult.Exception.Message);
                        }
                        Console.WriteLine("Execution time: " + testResult.ExecutionTime);
                    }

                    session.Store(ravenTestRun);
                    session.SaveChanges();
                }
                Console.WriteLine("===============================");
            }
        }
    }

    
}
