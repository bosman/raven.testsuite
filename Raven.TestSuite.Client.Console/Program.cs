using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new TestRunner.TestTests();
            var results = runner.RunTest();
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
            
            System.Console.ReadLine();
        }
    }

    
}
