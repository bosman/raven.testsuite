using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.TestSuite.Common.DatabaseObjects;

namespace Raven.TestSuite.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Console.WriteLine("starting test");

            //var domainContainer = new ClientWrapper.v2_5_2750.DomainContainer(
            //    "C:\\RavenDB-Build-2750\\Client\\Raven.Client.Lightweight.dll", "version1",
            //    "C:\\RavenDB-Build-2750\\Client",
            //    AppDomain.CurrentDomain.BaseDirectory);
            //var wrapper = domainContainer.Wrapper;
            //System.Console.WriteLine(wrapper.GetVersion());

            ////var strings = wrapper.GetSomeStrings();
            ////foreach (var s in strings)
            ////{
            ////    System.Console.WriteLine(s);
            ////}

            //var capitals = wrapper.QueryInSession<Country, List<string>>(
            //    x => x.Where(o => o.Area > 1000000).Select(a => a.Capital).ToList());

            //foreach (var capital in capitals)
            //{
            //    System.Console.WriteLine();
            //}

            //System.Console.ReadLine();

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
