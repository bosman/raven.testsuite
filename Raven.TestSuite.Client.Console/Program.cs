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

        public static void Main(string[] args)
        {
            SubscribeToApplicationExit();

            runner = new RavenTestRunner();
            var progressIndicator = new Progress<ProgressReport>(progressReport => Console.WriteLine(progressReport.Message));
            var cancellationtokenSource = new CancellationTokenSource();
            var token = cancellationtokenSource.Token;
            var task = runner.RunAllTests(progressIndicator, token, "C:\\RavenDB-Build-2750");
            task.ContinueWith(continuation =>
                {
                    if (continuation.IsCanceled)
                    {
                        Console.WriteLine("Tests cancelled");
                    }
                    else if (continuation.IsCompleted)
                    {
                        DisplayResults(continuation.Result);
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

        private static void DisplayResults(IEnumerable<TestResult> results)
        {
            Console.WriteLine("=====Results=====");
            foreach (var testResult in results)
            {
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
                Console.WriteLine("===============================");
            }
        }
    }

    
}
