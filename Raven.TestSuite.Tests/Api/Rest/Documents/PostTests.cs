namespace Raven.TestSuite.Tests.Api.Rest.Crud
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class PostTests : BaseRestApiTestGroup
    {
        public PostTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void CreateNewDocumentsTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost(Constants.DbUrl.Northwind + "/docs",
                    "{ FirstName: 'Bob', LastName: 'Smith', Address: '5 Elm St' }");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.NotNull(response.RavenJTokenWrapper.Value<string>("Key"));
                Assert.NotNull(response.RavenJTokenWrapper.Value<string>("ETag"));

                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/" + response.RavenJTokenWrapper.Value<string>("Key"));
                base.AssertNotNullGetResponse(response);
                Assert.Equal("Bob", response.RavenJTokenWrapper.Value<string>("FirstName"));
            });
        }

        [RavenRestApiTest]
        public void SendPostToInvalidUrlTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPost(Constants.DbUrl.Northwind + "/docs/orders/1",
                    "{ FirstName: 'Bob', LastName: 'Smith', Address: '5 Elm St' }");
                Assert.Equal(400, (int)response.RawResponse.StatusCode);
            });
        }
    }
}
