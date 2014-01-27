using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Raven.TestSuite.Common;
using Raven.TestSuite.Tests.Common.Commands;
using Xunit.Sdk;

namespace Raven.TestSuite.Tests.Common.Runner
{
    public class XunitRunner : TestClassCommand
    {
        private static Assembly testRunnerAssembly;

        public override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo testMethod)
        {
            yield return new RavenCommand(testMethod);
        }
    }
}