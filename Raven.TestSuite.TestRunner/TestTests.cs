using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.DatabaseObjects;
using Raven.TestSuite.Tests;

namespace Raven.TestSuite.TestRunner
{
    public class TestTests
    {
        public List<TestResult> RunTest()
        {
            System.Console.WriteLine("starting test");

            var domainContainer = new ClientWrapper.v2_5_2750.DomainContainer(
                "C:\\RavenDB-Build-2750\\Client\\Raven.Client.Lightweight.dll", "version1",
                "C:\\RavenDB-Build-2750\\Client",
                AppDomain.CurrentDomain.BaseDirectory);
            var wrapper = domainContainer.Wrapper;
            System.Console.WriteLine(wrapper.GetVersion());

            
            var someTestGroup = new InitialDevTests(wrapper);
            var testResults = someTestGroup.RunTests();


            return testResults;
        }
    }
}
