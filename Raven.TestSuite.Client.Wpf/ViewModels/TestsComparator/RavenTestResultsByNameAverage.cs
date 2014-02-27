using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Indexes;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class RavenTestResultsByNameAverage : AbstractTransformerCreationTask<RavenTestResult>
    {
        public class Result
        {
            public string TestName { get; set; }

            public TimeSpan? ExecutionTime { get; set; }
        }

        public RavenTestResultsByNameAverage()
        {
            TransformResults = testResults => testResults.GroupBy(x => x.TestName).Select(g => new Result
                {
                    TestName = g.Key,
                    ExecutionTime = TimeSpan.FromMilliseconds(g.Average(x => x.ExecutionTime.TotalMilliseconds))
                });
        }
    }
}
