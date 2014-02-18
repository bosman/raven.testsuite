using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.TestRunner;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestLibraryViewModel
    {
        private static RavenTestRunner runner;

        private ReadOnlyCollection<TestCategoryViewModel> testCategories;

        private ObservableCollection<string> logMessages;

        private bool isTestsRunning = false;
        private bool isTestStopping = false;
        private CancellationTokenSource cancellationTokenSource;

        public TestLibraryViewModel()
        {
            runner = new RavenTestRunner();
            LoadAllAvailableTests();
            logMessages = new ObservableCollection<string>();
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

        #region Commands

        public ICommand RunTestsCommand { get { return new DelegateCommand(OnRunTests, () => !this.isTestsRunning); } }
        public ICommand StopTestsCommand { get { return new DelegateCommand(OnStopTests, () => this.isTestsRunning && !this.isTestStopping); } }

        #endregion

        #region Command Handlers

        private void OnRunTests()
        {
            var testsToRun = GetTestFullNamesToRun();

            this.isTestsRunning = true;
            this.isTestStopping = false;
            this.logMessages.Clear();
            var progressIndicator = new Progress<ProgressReport>(OnTestRunProgressReport);
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var versionsList = new List<string> { "C:\\RavenDB-Build-2750" };
            //var versionsList = new List<string> { "C:\\RavenDB-Unstable-Build-2804" };
            //var versionsList = new List<string> { "C:\\RavenDB-Build-2750", "C:\\RavenDB-Unstable-Build-2804" };
            var testRunSetup = new TestRunSetup { RavenVersionPath = versionsList, DatabasePort = 8080, TestFullNamesToRun = testsToRun};
            var task = runner.RunAllTests(progressIndicator, token, testRunSetup);
            task.ContinueWith(continuation =>
            {
                this.isTestsRunning = false;
                if (continuation.IsCanceled)
                {
                    this.isTestStopping = false;
                    this.logMessages.Add("Tests cancelled");
                }
                else if (continuation.IsCompleted)
                {
                    //StoreAndDisplayResults(continuation.Result);
                }
            });
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
            this.isTestStopping = true;
            cancellationTokenSource.Cancel();
        }

        #endregion

        private void OnTestRunProgressReport(ProgressReport progressReport)
        {
            this.logMessages.Add(progressReport.Message);
        }
    }
}
