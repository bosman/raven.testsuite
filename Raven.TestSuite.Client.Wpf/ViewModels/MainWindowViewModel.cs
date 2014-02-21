using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.TestRunner;
using System.Linq;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class MainWindowViewModel
    {
        public TestLibraryViewModel TestLibraryViewModel { get; set; }

        public MainWindowViewModel()
        {
            this.TestLibraryViewModel = new TestLibraryViewModel();
        }
    }
}