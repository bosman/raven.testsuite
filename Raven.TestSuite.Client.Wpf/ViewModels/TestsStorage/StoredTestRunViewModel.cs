using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsStorage
{
    public class StoredTestRunViewModel : BaseViewModel
    {
        private readonly RavenTestRun ravenTestRun;

        public StoredTestRunViewModel(RavenTestRun ravenTestRun)
        {
            this.ravenTestRun = ravenTestRun;
        }

        public int Id { get { return ravenTestRun.Id; } }

        public string RavenVersion { get { return ravenTestRun.RavenVersion; } }

        public string WrapperVersion { get { return ravenTestRun.WrapperVersion; } }

        public DateTime StartedAt { get { return ravenTestRun.StartedAt; } }

        public DateTime StoppedAt { get { return ravenTestRun.StoppedAt; } }

        public TimeSpan DbServerStartupTime { get { return ravenTestRun.DbServerStartupTime; } }

        public string ExceptionMessage { get { return ravenTestRun.ExceptionMessage; } }

        public Type ExceptionType { get { return ravenTestRun.ExceptionType; } }
    }
}
