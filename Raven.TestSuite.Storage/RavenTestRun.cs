using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.Storage
{
    public class RavenTestRun
    {
        public static RavenTestRun FromTestRun(TestRun testRun)
        {
            return new RavenTestRun
                {
                    RavenVersion = testRun.RavenVersion,
                    StartedAt = testRun.StartedAt,
                    StoppedAt = testRun.StoppedAt,
                    RavenTestResultIds = new List<int>()
                };
        }

        public string RavenVersion { get; set; }

        public List<int> RavenTestResultIds { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime StoppedAt { get; set; }
    }
}
