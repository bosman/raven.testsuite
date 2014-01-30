using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Xunit;

namespace Raven.TestSuite.Tests.Api.Rest
{
    public class TemporaryDevelopmentTests : BaseTestGroup
    {
        public TemporaryDevelopmentTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        /*
        THESE TESTS ARE ONLY FOR THE FRAMEWORK DEVELOPMENT PURPOSES. SHOULD BE REMOVED WHEN NO LONGER NEEDED.
        */
        [RavenRestApiTest]
        public void SimpleRawGetTest()
        {
            this.wrapper.Execute(env =>
                {
                    var response = env.RawGet("http://localhost:8080/databases/World/docs/city/1989");
                    Assert.NotNull(response);
                });
        }

        [RavenRestApiTest]
        public void SimpleRawPutTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut("/databases/World/docs/testing/1", "{ FirstName: 'Bob', LastName: 'Smith', Address: '5 Elm St' }");
                Assert.NotNull(response);
            });
        }

        [RavenRestApiTest]
        public void SimpleRawPostTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost("databases/World/docs", "{ FirstName: 'John', LastName: 'Doe', Address: '5 Elm St' }");
                Assert.NotNull(response);
            });
        }

        [RavenRestApiTest]
        public void SimpleRawDeleteTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawDelete("http://localhost:8080/databases/World/docs/testing/3");
                Assert.NotNull(response);
            });
        }
    }
}
