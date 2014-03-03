using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Client.Wpf.ViewModels.Graphs
{
    public class TestNameAndTimeViewModel
    {
        public string TestName { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public double TimeSpanInMiliseconds {get { return ExecutionTime.TotalMilliseconds; }}
    }
}
