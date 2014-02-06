using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common
{
    public class TestRun
    {
        public TestRun()
        {
            this.StartedAt = DateTime.Now;
        }

        public string RavenVersion { get; set; }

        public string WrapperVersion { get; set; }

        public List<TestResult> TestResults { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime StoppedAt { get; set; }

        public Exception Exception { get; set; }

        public TimeSpan DbServerStartupTime { get; set; }
    }
}
