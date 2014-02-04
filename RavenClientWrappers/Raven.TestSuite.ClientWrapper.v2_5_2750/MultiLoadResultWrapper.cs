using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class MultiLoadResultWrapper : IMultiLoadResultWrapper
    {
        public List<RavenJObjectWrapper> Results { get; set; }

        public List<RavenJObjectWrapper> Includes { get; set; }

        public MultiLoadResultWrapper()
        {
            Results = new List<RavenJObjectWrapper>();
            Includes = new List<RavenJObjectWrapper>();
        }
    }
}
