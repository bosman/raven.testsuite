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
    public class TestsStorageViewModel : INotifyPropertyChanged
    {
        private RavenTestRun selectedTestRun;

        public RavenTestRun SelectedTestRun
        {
            get { return selectedTestRun; }
            set
            {
                if (selectedTestRun != value)
                {
                    selectedTestRun = value;
                    OnPropertyChanged("SelectedTestRun");
                    if (selectedTestRun != null)
                    {
                        OnSearchTestResults(selectedTestRun.Id);
                    }
                }
            }
        }

        public IDocumentStore DocumentStore { get; set; }

        public ObservableCollection<RavenTestRun> RavenTestRuns { get; set; }

        public ObservableCollection<RavenTestResult> RavenTestResults { get; set; }

        public ICommand SearchTestRunsCommand { get; set; }

        public TestsStorageViewModel()
        {
            RavenTestRuns = new ObservableCollection<RavenTestRun>();
            RavenTestResults = new ObservableCollection<RavenTestResult>();
            SearchTestRunsCommand = new DelegateCommand(OnSearchTestRuns);
        }

        public void OnSearchTestRuns()
        {
            using (var session = DocumentStore.OpenSession())
            {
                RavenTestRuns.Clear();
                session.Query<RavenTestRun>().ToList().ForEach(x => RavenTestRuns.Add(x));
            }
        }

        public void OnSearchTestResults(int testRunId)
        {
            using (var session = DocumentStore.OpenSession())
            {
                RavenTestResults.Clear();
                var testRun = session.Include<RavenTestRun>(x => x.RavenTestResultIds).Load(testRunId);
                foreach (var ravenTestResultId in testRun.RavenTestResultIds)
                {
                    RavenTestResults.Add(session.Load<RavenTestResult>(ravenTestResultId));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
