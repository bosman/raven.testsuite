namespace Raven.TestSuite.Common.Data
{
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using System.Collections.Generic;

    public class MultiLoadResultWrapper
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
