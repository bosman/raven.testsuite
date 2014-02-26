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
using Raven.Client.Linq;
using Raven.TestSuite.Client.Wpf.Helpers.Extensions;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class TestsComparatorViewModel : INotifyPropertyChanged
    {
        public IDocumentStore DocumentStore { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<RavenVersionViewModel> AvailableVersions { get; set; }

        public ICommand SearchCommand { get; set; }

        private RavenVersionViewModel leftSelectedVersion;
        public RavenVersionViewModel LeftSelectedVersion
        {
            get { return leftSelectedVersion; }
            set
            {
                if (leftSelectedVersion != value)
                {
                    leftSelectedVersion = value;
                    OnPropertyChanged("LeftSelectedVersion");
                    if (leftSelectedVersion != null)
                    {
                        RefreshLeftTestRuns(leftSelectedVersion.VersionName);
                    }
                }
            }
        }

        private RavenVersionViewModel rightSelectedVersion;
        public RavenVersionViewModel RightSelectedVersion
        {
            get { return rightSelectedVersion; }
            set
            {
                if (rightSelectedVersion != value)
                {
                    rightSelectedVersion = value;
                    OnPropertyChanged("RightSelectedVersion");
                    if (rightSelectedVersion != null)
                    {
                        RefreshRightTestRuns(rightSelectedVersion.VersionName);
                    }
                }
            }
        }

        public ObservableCollection<RavenTestRunViewModel> LeftTestRuns { get; set; }

        public ObservableCollection<RavenTestRunViewModel> RightTestRuns { get; set; }

        public ObservableCollection<TestComparisonItemViewModel> ComparisonResults { get; set; }

        public TestsComparatorViewModel()
        {
            AvailableVersions = new ObservableCollection<RavenVersionViewModel>();
            LeftTestRuns = new ObservableCollection<RavenTestRunViewModel>();
            RightTestRuns = new ObservableCollection<RavenTestRunViewModel>();
            ComparisonResults = new ObservableCollection<TestComparisonItemViewModel>();
            SearchCommand = new DelegateCommand(Search);
        }

        private void Search()
        {
            ComparisonResults.Clear();
            using (var session = DocumentStore.OpenSession())
            {
                var leftTestRunIds = LeftTestRuns.Where(x => x.IsChecked).Select(x => x.Id).ToList();
                var rightTestRunIds = RightTestRuns.Where(x => x.IsChecked).Select(x => x.Id).ToList();

                var leftResults = session.Query<RavenTestResult>()
                                     .Where(x => x.RavenTestRunId.In(leftTestRunIds))
                                     .TransformWith<RavenTestResultsByNameAverage, RavenTestResultsByNameAverage.Result>()
                                     .ToList();

                var rightResults = session.Query<RavenTestResult>()
                                     .Where(x => x.RavenTestRunId.In(rightTestRunIds))
                                     .TransformWith<RavenTestResultsByNameAverage, RavenTestResultsByNameAverage.Result>()
                                     .ToList();

                var results =
                    leftResults.FullOuterJoin(rightResults, x => x.TestName, y => y.TestName,
                                     (x, y, name) => new TestComparisonItemViewModel
                                         {
                                             Name = name,
                                             LeftExecutionTime = x.ExecutionTime,
                                             RightExecutionTime = y.ExecutionTime
                                         }, new RavenTestResultsByNameAverage.Result(), new RavenTestResultsByNameAverage.Result()).ToList();

                results.ForEach(ComparisonResults.Add);
            }
        }

        private void RefreshLeftTestRuns(string ravenVersion)
        {
            using (var session = DocumentStore.OpenSession())
            {
                LeftTestRuns.Clear();
                session.Query<RavenTestRun>()
                       .Where(x => x.RavenVersion == ravenVersion)
                       .ToList()
                       .ForEach(x => LeftTestRuns.Add(RavenTestRunViewModel.FromRavenTestRun(x)));
            }
        }

        private void RefreshRightTestRuns(string ravenVersion)
        {
            using (var session = DocumentStore.OpenSession())
            {
                RightTestRuns.Clear();
                session.Query<RavenTestRun>()
                       .Where(x => x.RavenVersion == ravenVersion)
                       .ToList()
                       .ForEach(x => RightTestRuns.Add(RavenTestRunViewModel.FromRavenTestRun(x)));
            }
        }

        public void RefreshAvailableVersions()
        {
            AvailableVersions.Clear();
            using (var session = DocumentStore.OpenSession())
            {
                session.Query<RavenTestRun>()
                       .Select(x => x.RavenVersion)
                       .Distinct()
                       .ToList()
                       .ForEach(x => AvailableVersions.Add(new RavenVersionViewModel { VersionName = x }));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
