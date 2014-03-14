namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class IndexesBasicOperationsTests : BaseRestApiTestGroup
    {
        public IndexesBasicOperationsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void CreateAndDeleteSimpleIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts",
                    "{ Map:'from product in docs.Products where product.Discontinued == true select new { product.Discontinued }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("discontinuedProducts", response.RavenJTokenWrapper.Value<string>("Index"));

                response = env.RawDelete(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts");
                Assert.Equal(204, (int)response.RawResponse.StatusCode);

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts");
                Assert.Equal(404, (int)response.RawResponse.StatusCode);
            });
        }
    }
}
