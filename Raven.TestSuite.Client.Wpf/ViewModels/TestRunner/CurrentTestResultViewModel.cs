using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Raven.TestSuite.Common;
using Raven.TestSuite.TestRunner;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestRunner
{
    public class CurrentTestResultViewModel
    {
        public string TestName { get; set; }

        public string HumanTestName
        {
            get { return TestName.Replace("Raven.TestSuite.Tests.", string.Empty); }
        }

        public Brush Color
        {
            get { return IsSuccess ? Brushes.PaleGreen : Brushes.Tomato; }
        }

        public string TestType { get; set; }

        public bool IsSuccess { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public Exception Exception { get; set; }

        public string RavenDllVersion { get; set; }

        public static CurrentTestResultViewModel FromTestResultProgressReport(TestResultProgressReport testResultPr)
        {
            var result = new CurrentTestResultViewModel
                {
                    TestName = testResultPr.TestResult.TestName,
                    TestType = testResultPr.TestResult.TestType,
                    IsSuccess = testResultPr.TestResult.IsSuccess,
                    ExecutionTime = testResultPr.TestResult.ExecutionTime,
                    Exception = testResultPr.TestResult.Exception,
                    RavenDllVersion = testResultPr.RavenVersion
                };
            return result;
        }
    }
}
