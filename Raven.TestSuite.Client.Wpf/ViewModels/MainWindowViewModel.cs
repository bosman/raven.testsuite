using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
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

        public IDocumentStore docStore;

        public MainWindowViewModel()
        {
            docStore = new Raven.Client.Embedded.EmbeddableDocumentStore
            {
                DataDirectory = "Data",
                UseEmbeddedHttpServer = true
            }.Initialize();
            this.TestLibraryViewModel = new TestLibraryViewModel {DocumentStore = this.docStore};
            this.TestsStorageViewModel = new TestsStorageViewModel {DocumentStore = this.docStore};
            this.TestsStorageViewModel.OnSearchTestRuns();
        }
    }
}