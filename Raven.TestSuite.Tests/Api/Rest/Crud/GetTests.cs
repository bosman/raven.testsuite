namespace Raven.TestSuite.Tests.Api.Rest.Crud
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Xunit;

    [RequiresFreshNorthwindDatabaseAttribute]
    public class GetTests : BaseRestApiTestGroup
    {
        public GetTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void GetNotExistingDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/docs/NotExisting/1");
                Assert.Null(response.RavenJTokenWrapper);
                Assert.Equal((int)response.RawResponse.StatusCode, 404);
            });
        }

        [RavenRestApiTest]
        public void GetExistingDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/1");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("Beverages", response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }
    }
}
