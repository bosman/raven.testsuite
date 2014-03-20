namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using System.Collections.Generic;
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

        [RavenRestApiTest]
        public void CreateAndQuerySimpleMapIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts2",
                    "{ Map:'from product in docs.Products select new { product.Discontinued }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("discontinuedProducts2", response.RavenJTokenWrapper.Value<string>("Index"));
                base.WaitForIndexes();

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts2", "query=Discontinued:True");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(8, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in results)
                {
                    Assert.True(item.Value<bool>("Discontinued"));
                }
            });
        }

        [RavenRestApiTest]
        public void CreateAndQuerySimpleMapReduceIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/ordersByCompany",
                    "{ Map:'from order in docs.Orders select new { order.Company, order.Freight }', " +
                    "Reduce:'from result in results group result by result.Company into g select new { Company = g.Key, Freight = g.Sum(x=>x.Freight) }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("ordersByCompany", response.RavenJTokenWrapper.Value<string>("Index"));
                base.WaitForIndexes();

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/ordersByCompany");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(89, response.RavenJTokenWrapper.Value<int>("TotalResults"));

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/ordersByCompany", "query=Company:companies/52");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(321.46, response.RavenJTokenWrapper.Value<float>("Company"));

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/ordersByCompany", "query=Company:companies/24");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(1654.41, response.RavenJTokenWrapper.Value<float>("Company"));
            });
        }

        [RavenRestApiTest]
        public void CreateAndQuerySimpleIndexWithPaging()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts3",
                    "{ Map:'from product in docs.Products select new { product.Discontinued }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("discontinuedProducts3", response.RavenJTokenWrapper.Value<string>("Index"));
                base.WaitForIndexes();

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts3", "query=Discontinued:True&start=0&pageSize=2");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(8, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                Assert.Equal(2, response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results").Length);

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts3", "query=Discontinued:True&start=2&pageSize=2");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(8, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                Assert.Equal(2, response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results").Length);

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts3", "query=Discontinued:True&start=4&pageSize=5");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(8, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                Assert.Equal(4, response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results").Length);
            });
        }

        [RavenRestApiTest]
        public void SimpleQueryDynamicIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Discontinued:True");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Discontinued:True");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(8, response.RavenJTokenWrapper.Value<int>("TotalResults"));
            });
        }
    }
}
