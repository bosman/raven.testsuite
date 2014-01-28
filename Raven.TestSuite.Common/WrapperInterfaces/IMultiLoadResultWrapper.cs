using System.Collections.Generic;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IMultiLoadResultWrapper
    {
        List<RavenJObjectWrapper> Results { get; set; }
        List<RavenJObjectWrapper> Includes { get; set; }
    }
}