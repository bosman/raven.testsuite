using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.Storage
{
    public class RavenTestResult
    {
        public static RavenTestResult FromTestResult(TestResult testResult)
        {
            return new RavenTestResult
                {
                    TestName = testResult.TestName,
                    ExecutionTime = testResult.ExecutionTime,
                    IsSuccess = testResult.IsSuccess,
                    ExceptionMessage = testResult.Exception != null ? testResult.Exception.Message : string.Empty,
                    ExceptionType = testResult.Exception != null ? testResult.Exception.GetType() : null
                };
        }

        public int Id { get; set; }

        public string TestName { get; set; }

        public bool IsSuccess { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public string ExceptionMessage { get; set; }

        public Type ExceptionType { get; set; }
    }
}
