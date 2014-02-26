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
                    WrapperVersion = testRun.WrapperVersion,
                    StartedAt = testRun.StartedAt,
                    StoppedAt = testRun.StoppedAt,
                    ExceptionMessage = testRun.Exception != null ? testRun.Exception.Message : string.Empty,
                    ExceptionType = testRun.Exception != null ? testRun.Exception.GetType() : null,
                };
        }

        public int Id { get; set; }

        public string RavenVersion { get; set; }

        public string WrapperVersion { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime StoppedAt { get; set; }

        public string ExceptionMessage { get; set; }

        public Type ExceptionType { get; set; }
    }
}
