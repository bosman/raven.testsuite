using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class TestRuns_TestResultsIndex : AbstractIndexCreationTask<RavenTestRun>
    {
        public class Result
        {
            public string TestRunId { get; set; }
            public string TestName { get; set; }
            public TimeSpan ExecutionTime { get; set; }
            public int Count { get; set; }
        }

        public TestRuns_TestResultsIndex()
        {
            Map = testRuns => from testRun in testRuns
                              select new
                                  {
                                      TestRunId = testRun.Id,
                                  };
        }
    }
}
