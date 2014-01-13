using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects;
using Xunit;

namespace Raven.TestSuite.Tests
{
    public class InitialDevTests : TestGroupBase
    {
        private IRavenClientWrapper wrapper;

        public InitialDevTests(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        [RavenTest]
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

        [RavenTest]
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
