using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Raven.TestSuite.Common.Attributes;

namespace Raven.TestSuite.Common
{
    public class TestGroupBase
    {
        public List<TestResult> RunTests()
        {
            var testMethods = this.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(RavenTestAttribute), false).Length > 0)
                .ToList();

            var testResults = new List<TestResult>();
            Stopwatch watch = new Stopwatch();

            foreach (var testMethod in testMethods)
            {
                watch.Reset();
                var result = new TestResult();
                result.TestName = testMethod.Name;
                watch.Start();
                try
                {
                    testMethod.Invoke(this, new object[0]);
                    result.IsSuccess = true;
                }
                catch (TargetInvocationException ex)
                {
                    result.IsSuccess = false;
                    result.Exception = ex.InnerException;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Exception = ex;
                }
                finally
                {
                    watch.Stop();
                    result.ExecutionTime = watch.Elapsed;
                }
                testResults.Add(result);
            }

            return testResults;
        }
    }
}
