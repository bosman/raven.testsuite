using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.Client.Wpf.Models
{
    public class TestModel : NotificationObject
    {
        public string FullName { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        private TestResult lastTestResult;
        public TestResult LastTestResult
        {
            get { return lastTestResult; }
            set
            {
                if (lastTestResult != value)
                {
                    lastTestResult = value;
                    RaisePropertyChanged(() => LastTestResult);
                }
            }
        }
    }
}
