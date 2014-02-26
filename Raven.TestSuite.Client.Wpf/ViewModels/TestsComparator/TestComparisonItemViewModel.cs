using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class TestComparisonItemViewModel
    {
        public string Name { get; set; }

        public TimeSpan LeftExecutionTime { get; set; }

        public TimeSpan RightExecutionTime { get; set; }
    }
}
