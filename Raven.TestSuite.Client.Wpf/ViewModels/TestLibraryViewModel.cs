using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.Common;
using Raven.TestSuite.TestRunner;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestLibraryViewModel : INotifyPropertyChanged
    {
        private static RavenTestRunner runner;

        private ReadOnlyCollection<TestCategoryViewModel> testCategories;

        private ObservableCollection<string> logMessages;

        private ObservableCollection<TestResult> testResults; 

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

        public TestLibraryViewModel()
        {
            RunTestsCommand = new DelegateCommand(OnRunTests, () => !this.IsTestsRunning);
            StopTestsCommand = new DelegateCommand(OnStopTests, () => this.IsTestsRunning && !this.IsTestStopping);
            runner = new RavenTestRunner();
            LoadAllAvailableTests();
            logMessages = new ObservableCollection<string>();
            testResults = new ObservableCollection<TestResult>();
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

        public ObservableCollection<TestResult> TestResults
        {
            get { return testResults; }
        }

        #region Command Handlers

        private async void OnRunTests()
        {
            var testsToRun = GetTestFullNamesToRun();

            IsTestsRunning = true;
            IsTestStopping = false;
            this.logMessages.Clear();
            this.testResults.Clear();
            var progressIndicator = new Progress<ProgressReport>(OnTestRunProgressReport);
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var versionsList = new List<string> { "C:\\RavenDB-Build-2750" };
            //var versionsList = new List<string> { "C:\\RavenDB-Unstable-Build-2804" };
            //var versionsList = new List<string> { "C:\\RavenDB-Build-2750", "C:\\RavenDB-Unstable-Build-2804" };
            var testRunSetup = new TestRunSetup { RavenVersionPath = versionsList, DatabasePort = 8080, TestFullNamesToRun = testsToRun};
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
            this.logMessages.Add("Finished");
            CommandManager.InvalidateRequerySuggested();
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
                foreach (var testCategory in TestCategories)
                {
                    testCategory.UpdateLastTestResult(testResultProgressReport.TestResult);
                }
                testResults.Add(testResultProgressReport.TestResult);
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
