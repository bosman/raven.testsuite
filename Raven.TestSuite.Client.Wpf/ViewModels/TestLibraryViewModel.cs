using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Raven.Client;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.Client.Wpf.ViewModels.TestRunner;
using Raven.TestSuite.Common;
using Raven.TestSuite.Storage;
using Raven.TestSuite.TestRunner;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestLibraryViewModel : INotifyPropertyChanged
    {
        private static RavenTestRunner runner;
 
        public event EventHandler TestRunsStored;

        protected virtual void OnTestRunsStored()
        {
            EventHandler handler = TestRunsStored;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public IDocumentStore DocumentStore { get; set; }

        private ReadOnlyCollection<TestCategoryViewModel> testCategories;

        private ObservableCollection<string> logMessages;

        private ObservableCollection<CurrentTestResultViewModel> currentTestResults;

        public ObservableCollection<VersionFolderViewModel> VersionFolders { get; set; }

        private bool isTestsRunning = false;
        public bool IsTestsRunning
        {
            get { return isTestsRunning; }
            set
            {
                if (isTestsRunning != value)
                {
                    isTestsRunning = value;
                    this.OnPropertyChanged("IsTestsRunning");
                }
            }
        }

        private bool isTestStopping = false;
        public bool IsTestStopping
        {
            get { return isTestStopping; }
            set
            {
                if (isTestStopping != value)
                {
                    isTestStopping = value;
                    this.OnPropertyChanged("IsTestStopping");
                }
            }
        }

        private CancellationTokenSource cancellationTokenSource;

        public ICommand RunTestsCommand { get; set; }
        public ICommand StopTestsCommand { get; set; }
        public ICommand AddVersionFolder { get; set; }

        public TestLibraryViewModel()
        {
            RunTestsCommand = new DelegateCommand(OnRunTests, () => !this.IsTestsRunning);
            StopTestsCommand = new DelegateCommand(OnStopTests, () => this.IsTestsRunning && !this.IsTestStopping);
            AddVersionFolder = new DelegateCommand(OnAddVersionFolder, () => !this.isTestsRunning);
            runner = new RavenTestRunner();
            LoadAllAvailableTests();
            logMessages = new ObservableCollection<string>();
            currentTestResults = new ObservableCollection<CurrentTestResultViewModel>();
            VersionFolders = new ObservableCollection<VersionFolderViewModel>();
        }

        private void LoadAllAvailableTests()
        {
            var testCategories = runner.GetAllRavenTests()
                                       .GroupBy(x => x.Category)
                                       .Select(y => new TestCategoryModel
                                           {
                                               Name = y.Key.Name,
                                               TestGroups = y.Select(tg => new TestGroupModel
                                                   {
                                                       Name = tg.GroupType.Name,
                                                       Tests = tg.Tests.Select(t => new TestModel
                                                           {
                                                               Name = t.Name,
                                                               FullName = tg.GroupType.FullName + "." + t.Name
                                                           }).ToList()
                                                   }).ToList()
                                           }).ToList();

            this.testCategories =
                new ReadOnlyCollection<TestCategoryViewModel>(testCategories.Select(x => new TestCategoryViewModel(x)).ToList());
        }

        public ReadOnlyCollection<TestCategoryViewModel> TestCategories
        {
            get { return testCategories; }
        }

        public ObservableCollection<string> LogMessages
        {
            get { return logMessages; }
        }

        public ObservableCollection<CurrentTestResultViewModel> CurrentTestResults
        {
            get { return currentTestResults; }
        }

        #region Command Handlers

        private void OnAddVersionFolder()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                VersionFolders.Add(new VersionFolderViewModel { Path = dialog.SelectedPath, IsSelected = true });
            }
        }

        private async void OnRunTests()
        {
            var testsToRun = GetTestFullNamesToRun();

            IsTestsRunning = true;
            IsTestStopping = false;
            this.logMessages.Clear();
            this.currentTestResults.Clear();
            var progressIndicator = new Progress<ProgressReport>(OnTestRunProgressReport);
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var versionsList = VersionFolders.Where(vf => vf.IsSelected).Select(vf => vf.Path).ToList();
            var testRunSetup = new TestRunSetup { RavenVersionPath = versionsList, DatabasePort = 8082, TestFullNamesToRun = testsToRun };
            var task = runner.RunAllTests(progressIndicator, token, testRunSetup);
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                if (task.IsCanceled)
                {
                    IsTestStopping = false;
                    this.logMessages.Add("Tests cancelled");
                }
                else
                {
                    this.logMessages.Add("Error: " + ex.Message);
                }
            }
            IsTestsRunning = false;
            if (task.IsCompleted)
            {
                StoreResults(task.Result);
                OnTestRunsStored();
            }
            this.logMessages.Add("Finished");
            CommandManager.InvalidateRequerySuggested();
        }

        private void StoreResults(IEnumerable<TestRun> testRuns)
        {
            foreach (var testRun in testRuns)
            {
                var ravenTestRun = RavenTestRun.FromTestRun(testRun);
                using (var session = DocumentStore.OpenSession())
                {
                    session.Store(ravenTestRun);
                    foreach (var testResult in testRun.TestResults)
                    {
                        var ravenTestResult = RavenTestResult.FromTestResult(testResult);
                        ravenTestResult.RavenTestRunId = ravenTestRun.Id;
                        session.Store(ravenTestResult);
                    }
                    
                    session.SaveChanges();
                }
            }
        }

        private IList<string> GetTestFullNamesToRun()
        {
            var results = new List<string>();
            foreach (var testCategory in TestCategories)
            {
                results.AddRange(testCategory.Children.SelectMany(GetTestFullNamesToRun).ToList());
            }
            return results;
        }

        private IList<string> GetTestFullNamesToRun(TreeViewItemViewModel treeViewItemViewModel)
        {
            var results = new List<string>();
            var testViewModel = treeViewItemViewModel as TestViewModel;
            if (testViewModel != null)
            {
                if (testViewModel.IsChecked)
                {
                    results.Add(testViewModel.TestFullName);
                }
            }
            else
            {
                results.AddRange(treeViewItemViewModel.Children.SelectMany(GetTestFullNamesToRun).ToList());
            }
            return results;
        }

        private void OnStopTests()
        {
            IsTestStopping = true;
            cancellationTokenSource.Cancel();
        }

        #endregion

        private void OnTestRunProgressReport(ProgressReport progressReport)
        {
            var testResultProgressReport = progressReport as TestResultProgressReport;
            if (testResultProgressReport != null)
            {
                currentTestResults.Add(CurrentTestResultViewModel.FromTestResultProgressReport(testResultProgressReport));
            }
            this.logMessages.Add(progressReport.Message);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
