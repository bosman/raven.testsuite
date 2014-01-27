using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml;
using Raven.TestSuite.Common;
using Raven.TestSuite.TestRunner;
using Xunit.Sdk;

namespace Raven.TestSuite.Tests.Common.Commands
{
    public class RavenCommand : FactCommand, IRavenCommand
    {

        public RavenCommand(IMethodInfo methodInfo)
            : base(methodInfo)
        {
        }

        public override MethodResult Execute(object testClass)
        {
            if (testClass is BaseTestGroup)
            {
                var baseTest = testClass as BaseTestGroup;
                var runner = new RavenTestRunner();
                try
                {
                    var progressIndicator = new Progress<ProgressReport>(progressReport => Console.WriteLine(progressReport.Message));
                    //TODO: test result
                    runner.RunTests(progressIndicator, CancellationToken.None, "C:\\RavenDB-Build-2750", info => true).Wait();
                    return new PassedResult(testMethod, testMethod.Name);
                }
                catch (Exception e)
                {
                    return new FailedResult(testMethod, e, testMethod.Name);
                }
            }
            return null;
        }
    }
}