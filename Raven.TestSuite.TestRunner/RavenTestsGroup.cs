using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.TestRunner
{
    public class RavenTestsGroup
    {
        public Type GroupType { get; set; }

        public Type Category { get; set; }

        public IList<MethodInfo> Tests { get; set; } 
    }
}
