using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator;
using Raven.TestSuite.Client.Wpf.ViewModels.TestsStorage;
using Raven.TestSuite.TestRunner;
using System.Linq;
using Raven.Client;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class MainWindowViewModel
    {
        public TestLibraryViewModel TestLibraryViewModel { get; set; }

        public TestsStorageViewModel TestsStorageViewModel { get; set; }

        public TestsComparatorViewModel TestsComparatorViewModel { get; set; }

        public IDocumentStore docStore;

        public MainWindowViewModel()
        {
            docStore = new Raven.Client.Embedded.EmbeddableDocumentStore
            { 
                DataDirectory = "Data",
                UseEmbeddedHttpServer = true
            }.Initialize();
            CreateIndexesAndTransformers(docStore);
            this.TestLibraryViewModel = new TestLibraryViewModel {DocumentStore = this.docStore};
            this.TestLibraryViewModel.TestRunsStored += TestLibraryViewModel_InsertedItemsToDb;
            this.TestsStorageViewModel = new TestsStorageViewModel {DocumentStore = this.docStore};
            this.TestsComparatorViewModel = new TestsComparatorViewModel {DocumentStore = this.docStore};
            RunRefreshCommandsInStorage();
            RunRefreshCommandsInComparator();
        }

        protected void TestLibraryViewModel_InsertedItemsToDb(object sender, EventArgs e)
        {
            RunRefreshCommandsInStorage();
            RunRefreshCommandsInComparator();
        }

        private void CreateIndexesAndTransformers(IDocumentStore documentStore)
        {
            new RavenTestResultsByNameAverage().Execute(docStore);
        }

        private void RunRefreshCommandsInStorage()
        {
            if (TestsStorageViewModel.RefreshTestRunsCommand.CanExecute(null))
            {
                this.TestsStorageViewModel.RefreshTestRunsCommand.Execute(null);
            }
        }

        private void RunRefreshCommandsInComparator()
        {
            if (TestsComparatorViewModel.RefreshAvailableVersionsCommand.CanExecute(null))
            {
                this.TestsComparatorViewModel.RefreshAvailableVersionsCommand.Execute(null);
            }
        }
    }
}