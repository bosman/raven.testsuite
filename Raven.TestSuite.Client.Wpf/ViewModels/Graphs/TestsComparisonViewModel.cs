using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Client.Wpf.ViewModels.Graphs
{
    public class TestsComparisonViewModel
    {
        public TestsComparisonViewModel()
        {
            LeftData = new ObservableCollection<TestNameAndTimeViewModel>();
            RightData = new ObservableCollection<TestNameAndTimeViewModel>();
        }

        public ObservableCollection<TestNameAndTimeViewModel> LeftData { get; set; }

        public ObservableCollection<TestNameAndTimeViewModel> RightData { get; set; }

        public string LeftVersion { get; set; }

        public string RightVersion { get; set; }
    }
}
