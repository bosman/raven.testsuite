using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Raven.Client;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsStorage
{
    public class TestsStorageViewModel : BaseViewModel
    {
        private StoredTestRunViewModel selectedTestRun;

        public StoredTestRunViewModel SelectedTestRun
        {
            get { return selectedTestRun; }
            set
            {
                if (selectedTestRun != value)
                {
                    selectedTestRun = value;
                    RaisePropertyChanged(() => SelectedTestRun);
                    if (selectedTestRun != null)
                    {
                        OnSearchTestResults(selectedTestRun.Id);
                    }
                }
            }
        }

        public IDocumentStore DocumentStore { get; set; }

        public ObservableCollection<StoredTestRunViewModel> RavenTestRuns { get; set; }

        public ObservableCollection<RavenTestResult> RavenTestResults { get; set; }

        public ICommand RefreshTestRunsCommand { get; set; }

        public TestsStorageViewModel()
        {
            RavenTestRuns = new ObservableCollection<StoredTestRunViewModel>();
            RavenTestResults = new ObservableCollection<RavenTestResult>();
            RefreshTestRunsCommand = new DelegateCommand(OnSearchTestRuns);
        }

        private void OnSearchTestRuns()
        {
            using (var session = DocumentStore.OpenSession())
            {
                RavenTestRuns.Clear();
                session.Query<RavenTestRun>().Customize(x => x.WaitForNonStaleResultsAsOfNow()).ToList().ForEach(x => RavenTestRuns.Add(new StoredTestRunViewModel(x)));
            }
            if (!RavenTestRuns.Contains(SelectedTestRun))
            {
                SelectedTestRun = null;
            }
        }

        private void OnSearchTestResults(int testRunId)
        {
            using (var session = DocumentStore.OpenSession())
            {
                RavenTestResults.Clear();
                var testResults = session.Query<RavenTestResult>().Where(x => x.RavenTestRunId == testRunId).ToList();
                testResults.ForEach(r => RavenTestResults.Add(r));
            }
        }
    }
}
