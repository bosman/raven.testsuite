using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common
{
    public class TestRunWithTestResults
    {
        public TestRun TestRun { get; set; }

        public IList<TestResult> TestResults { get; set; } 
    }
}
