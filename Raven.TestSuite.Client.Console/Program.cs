using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raven.TestSuite.Common;
using Raven.TestSuite.TestRunner;

namespace Raven.TestSuite.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new RavenTestRunner();
            var progressIndicator = new Progress<ProgressReport>(progressReport => System.Console.WriteLine(progressReport.Message));
            var cancellationtokenSource = new CancellationTokenSource();
            var token = cancellationtokenSource.Token;
            var task = runner.RunAllTests(progressIndicator, token);
            task.ContinueWith(continuation =>
                {
                    if (continuation.IsCanceled)
                    {
                        System.Console.WriteLine("Tests cancelled");
                    }
                    else if (continuation.IsCompleted)
                    {
                        DisplayResults(continuation.Result);
                    }
                    
                });

            var shouldQuit = false;
            while (!shouldQuit)
            {
                var key = System.Console.ReadKey();
                switch (key.Key)
                {
                        case ConsoleKey.T:
                            System.Console.WriteLine("Test");
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

        private static void DisplayResults(List<TestResult> results)
        {
            System.Console.WriteLine("=====Results=====");
            foreach (var testResult in results)
            {
                System.Console.WriteLine("Test name: " + testResult.TestName);
                if (testResult.IsSuccess)
                {
                    System.Console.WriteLine("Success");
                }
                else
                {
                    System.Console.WriteLine("Failure: " + testResult.Exception.Message);
                }
                System.Console.WriteLine("Execution time: " + testResult.ExecutionTime);
                System.Console.WriteLine("===============================");
            }
        }
    }

    
}
