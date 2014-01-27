using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.Common.Runner;
using Xunit;

namespace Raven.TestSuite.Tests.Common
{
    [RunWithRaven]
    public class BaseTestGroup
    {
        public IRavenClientWrapper wrapper { get; set; }

       
    }
}
