namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.ClientWrapper.v2_5_2750;
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using System.Collections.Generic;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class BulkDocsEndpointTests : BaseRestApiTestGroup
    {
        public BulkDocsEndpointTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void BatchAllAllowedOperationsInSingleRequestTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost(Constants.DbUrl.Northwind + "/bulk_docs",
                    "[{ Method: 'PUT', Document: { name: 'TestName' }, Metadata: {}, Key: 'BatchAllAllowedOperationsInSingleRequestTest'}," +
                    "{ Method: 'DELETE', Key: 'categories/1' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/BatchAllAllowedOperationsInSingleRequestTest");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("TestName", response.RavenJTokenWrapper.Value<string>("name"));

                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/1");
                Assert.Equal(404, (int)response.RawResponse.StatusCode);
            });
        }
    }
}
