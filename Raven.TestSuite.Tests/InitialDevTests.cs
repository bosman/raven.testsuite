using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects;
using System.Linq;
using Xunit;

namespace Raven.TestSuite.Tests
{
    public class InitialDevTests
    {
        private IRavenClientWrapper wrapper;

        public InitialDevTests(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        [RavenDotNetApiTest]
        public void SimpleTest1()
        {
            wrapper.Execute(store =>
                {
                    using (var session = store.OpenSession())
                    {
                        Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) > 0);
                    }
                });
        }

        [RavenDotNetApiTest]
        public void SimpleFailingTest1()
        {
            wrapper.Execute(store =>
            {
                using (var session = store.OpenSession())
                {
                    Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) < 0);
                }
            });
        }
    }
}
