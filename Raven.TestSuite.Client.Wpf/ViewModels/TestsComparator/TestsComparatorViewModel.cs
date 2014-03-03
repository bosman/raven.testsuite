using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Raven.Client;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.ViewModels.Graphs;
using Raven.TestSuite.Client.Wpf.Views.Graphs;
using Raven.TestSuite.Storage;
using Raven.Client.Linq;
using Raven.TestSuite.Client.Wpf.Helpers.Extensions;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class TestsComparatorViewModel : BaseViewModel
    {
        public IDocumentStore DocumentStore { get; set; }

        public ObservableCollection<RavenVersionViewModel> AvailableVersions { get; set; }

        public ICommand RefreshAvailableVersionsCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowResultsAsGraph { get; set; }

        private RavenVersionViewModel leftSelectedVersion;
        public RavenVersionViewModel LeftSelectedVersion
        {
            get { return leftSelectedVersion; }
            set
            {
                if (leftSelectedVersion != value)
                {
                    leftSelectedVersion = value;
                    RaisePropertyChanged(() => LeftSelectedVersion);
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
                    RaisePropertyChanged(() => RightSelectedVersion);
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
            RefreshAvailableVersionsCommand = new DelegateCommand(OnRefreshAvailableVersions);
            ShowResultsAsGraph = new DelegateCommand(OnShowResultsAsGraph);
        }

        private void Search()
        {
            ComparisonResults.Clear();
            using (var session = DocumentStore.OpenSession())
            {
                var leftTestRunIds = LeftTestRuns.Where(x => x.IsChecked).Select(x => x.Id).ToList();
                var rightTestRunIds = RightTestRuns.Where(x => x.IsChecked).Select(x => x.Id).ToList();

                var leftResults = session.Query<RavenTestResult>()
                                     .Where(x => x.RavenTestRunId.In(leftTestRunIds) && x.IsSuccess)
                                     .TransformWith<RavenTestResultsByNameAverage, RavenTestResultsByNameAverage.Result>()
                                     .ToList();

                var rightResults = session.Query<RavenTestResult>()
                                     .Where(x => x.RavenTestRunId.In(rightTestRunIds) && x.IsSuccess)
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
                       .Customize(x => x.WaitForNonStaleResultsAsOfNow())
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
                       .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                       .Where(x => x.RavenVersion == ravenVersion)
                       .ToList()
                       .ForEach(x => RightTestRuns.Add(RavenTestRunViewModel.FromRavenTestRun(x)));
            }
        }

        private void OnShowResultsAsGraph()
        {
            var window = new TestsComparison { Owner = Application.Current.MainWindow };
            var leftData =
                ComparisonResults.Where(x => x.LeftExecutionTime.HasValue)
                                 .Select(
                                     x =>
                                     new TestNameAndTimeViewModel
                                         {
                                             TestName = x.Name,
                                             ExecutionTime = x.LeftExecutionTime.Value
                                         })
                                 .ToList();
            var rightData =
                ComparisonResults.Where(x => x.RightExecutionTime.HasValue)
                                 .Select(
                                     x =>
                                     new TestNameAndTimeViewModel
                                     {
                                         TestName = x.Name,
                                         ExecutionTime = x.RightExecutionTime.Value
                                     })
                                 .ToList();
            var viewModel = new TestsComparisonViewModel
                {
                    LeftData = new ObservableCollection<TestNameAndTimeViewModel>(leftData),
                    RightData = new ObservableCollection<TestNameAndTimeViewModel>(rightData),
                    LeftVersion = string.Format("Left ({0})", LeftSelectedVersion.VersionName),
                    RightVersion = string.Format("Right ({0})", RightSelectedVersion.VersionName)
                };
            window.DataContext = viewModel;
            window.Show();
        }

        private void OnRefreshAvailableVersions()
        {
            AvailableVersions.Clear();
            using (var session = DocumentStore.OpenSession())
            {
                session.Query<RavenTestRun>()
                       .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                       .Select(x => x.RavenVersion)
                       .Distinct()
                       .ToList()
                       .ForEach(x => AvailableVersions.Add(new RavenVersionViewModel { VersionName = x }));
            }
            if (!AvailableVersions.Contains(LeftSelectedVersion))
            {
                LeftSelectedVersion = null;
            }
            if (!AvailableVersions.Contains(RightSelectedVersion))
            {
                RightSelectedVersion = null;
            }
        }
    }
}
