namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class MutlipleDocumentsOperationsTests : BaseRestApiTestGroup
    {
        public MutlipleDocumentsOperationsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void GetMultipleDocumentsTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost(Constants.DbUrl.Northwind + "/queries",
                    "['categories/1', 'categories/2']");
                base.AssertNotNullGetResponse(response);
                var results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                Assert.Equal(2, results.Length);
                Assert.Equal("Beverages", results[0].Value<string>("Name"));
                Assert.Equal("Condiments", results[1].Value<string>("Name"));


                response = env.RawPost(Constants.DbUrl.Northwind + "/queries",
                    "['categories/1', 'something', 'categories/2']");
                base.AssertNotNullGetResponse(response);
                results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                Assert.Equal(2, results.Length);
                Assert.Equal("Beverages", results[0].Value<string>("Name"));
                Assert.Equal("Condiments", results[1].Value<string>("Name"));

                response = env.RawGet(Constants.DbUrl.Northwind + "/queries", "id=categories/1&id=categories/2");
                base.AssertNotNullGetResponse(response);
                results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                Assert.Equal(2, results.Length);
                Assert.Equal("Beverages", results[0].Value<string>("Name"));
                Assert.Equal("Condiments", results[1].Value<string>("Name"));
            });
        }

        [RavenRestApiTest]
        public void GetMultipleDocumentsWithIncludesTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost(Constants.DbUrl.Northwind + "/queries",
                    "['orders/1', 'orders/2']", "include=Company&include=Employee");
                base.AssertNotNullGetResponse(response);

                var results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                var includes = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Includes");

                Assert.Equal(2, results.Length);
                Assert.Equal("companies/85", results[0].Value<string>("Company"));
                Assert.Equal("employees/5", results[0].Value<string>("Employee"));
                Assert.Equal("companies/79", results[1].Value<string>("Company"));
                Assert.Equal("employees/6", results[1].Value<string>("Employee"));

                Assert.Equal(4, includes.Length);
                Assert.Equal("Vins et alcools Chevalier", includes[0].Value<string>("Name"));
                Assert.Equal("Buchanan", includes[1].Value<string>("LastName"));
                Assert.Equal("Toms Spezialitäten", includes[2].Value<string>("Name"));
                Assert.Equal("Suyama", includes[3].Value<string>("LastName"));



                response = env.RawGet(Constants.DbUrl.Northwind + "/queries", "id=orders/1&id=orders/2&include=Company&include=Employee");
                base.AssertNotNullGetResponse(response);

                results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                includes = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Includes");

                Assert.Equal(2, results.Length);
                Assert.Equal("companies/85", results[0].Value<string>("Company"));
                Assert.Equal("employees/5", results[0].Value<string>("Employee"));
                Assert.Equal("companies/79", results[1].Value<string>("Company"));
                Assert.Equal("employees/6", results[1].Value<string>("Employee"));

                Assert.Equal(4, includes.Length);
                Assert.Equal("Vins et alcools Chevalier", includes[0].Value<string>("Name"));
                Assert.Equal("Buchanan", includes[1].Value<string>("LastName"));
                Assert.Equal("Toms Spezialitäten", includes[2].Value<string>("Name"));
                Assert.Equal("Suyama", includes[3].Value<string>("LastName"));
            });
        }

        [RavenRestApiTest]
        public void SetBasedDeletesTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts",
                    "{ Map:'from product in docs.Products select new { product.Discontinued }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("discontinuedProducts", response.RavenJTokenWrapper.Value<string>("Index"));

                response = env.RawDelete(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts",
                    "query=Discontinued:True");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/discontinuedProducts", "query=Discontinued:True");
                Assert.Equal(0, response.RavenJTokenWrapper.Value<int>("TotalResults"));
            });
        }

        [RavenRestApiTest]
        public void SetBasedUpdatesTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/indexes/PricePerUser",
                    "{ Map:'from product in docs.Products select new { product.PricePerUser }' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("PricePerUser", response.RavenJTokenWrapper.Value<string>("Index"));

                env.RawPatch(Constants.DbUrl.Northwind + "/indexes/PricePerUser",
                    "[{ Type: 'Set', Name: 'Name', Value: 'SetBasedUpdatesTest'}]",
                    "query=PricePerUser:18");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/PricePerUser", "query=PricePerUser:18");

                var results = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in results)
                {
                    Assert.Equal("SetBasedUpdatesTest", item.Value<string>("Name"));
                }
            });
        }
    }
}
