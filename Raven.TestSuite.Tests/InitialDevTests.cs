using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects;
using System.Linq;
using Xunit;

namespace Raven.TestSuite.Tests
{
    public class InitialDevTests : BaseTestGroup
    {
        public InitialDevTests(IRavenClientWrapper wrapper) : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        [PreinitializeDatabase("World")]
        public void SimpleTest1()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore("World").Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) > 0);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        [PreinitializeDatabase("World")]
        public void SimpleFailingTest1()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore("World").Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) < 0);
                    }
                }
            });
        }
    }
}
