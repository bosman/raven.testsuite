using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.TestRunner
{
    public class TestRunSetup
    {
        public TestRunSetup()
        {
            this.RavenVersionPath = new List<string>();
        }

        public List<string> RavenVersionPath { get; set; }
    }
}
