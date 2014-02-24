using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.TestRunner
{
    public class TestResultProgressReport : ProgressReport
    {
        public string RavenVersion { get; set; }

        public TestResult TestResult { get; set; }
    }
}
