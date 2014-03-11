namespace Raven.TestSuite.Tests.Api.Rest.Crud
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using System.Collections.Generic;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class PutTests : BaseRestApiTestGroup
    {
        public PutTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void PutToInvalidUrlTest()
        {
            this.wrapper.Execute(env => 
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/docs", "{}");
                Assert.Null(response.RavenJTokenWrapper);
                Assert.Equal(400, (int)response.RawResponse.StatusCode);
                
                response = env.RawPut(Constants.DbUrl.Northwind + "/docs/", "{}");
                Assert.Null(response.RavenJTokenWrapper);
                Assert.Equal(400, (int)response.RawResponse.StatusCode);
            });
        }

        [RavenRestApiTest]
        public void CreateEmptyDocumentTest()
        {
            this.wrapper.Execute(env => 
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/docs/CreateEmptyDocumentTest/1", "{}");
                Assert.NotNull(response.RavenJTokenWrapper);
                Assert.NotNull(response.RavenJTokenWrapper.Value<string>("ETag"));
                Assert.Equal("CreateEmptyDocumentTest/1", response.RavenJTokenWrapper.Value<string>("Key"));
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
            });
        }

        [RavenRestApiTest]
        public void ReplaceDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/docs/ReplaceDocumentTest/1", "{a:1}");
                response = env.RawPut(Constants.DbUrl.Northwind + "/docs/ReplaceDocumentTest/1", "{b:2}");
                Assert.NotNull(response.RavenJTokenWrapper);
                Assert.NotNull(response.RavenJTokenWrapper.Value<string>("ETag"));
                Assert.Equal("ReplaceDocumentTest/1", response.RavenJTokenWrapper.Value<string>("Key"));
                Assert.Equal(201, (int)response.RawResponse.StatusCode);

                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/ReplaceDocumentTest/1");
                Assert.Equal("2", response.RavenJTokenWrapper.Value<string>("b"));
            });
        }

        [RavenRestApiTest]
        public void SetMetadataTest()
        {
            this.wrapper.Execute(env =>
            {
                var headers = new Dictionary<string,List<string>>();
                headers.Add("Some-Raven-Metadata-Key", new List<string>(new string[] {"some-raven-metadata-value"}));

                var response = env.RawPut(Constants.DbUrl.Northwind + "/docs/SetMetadataTest/1", "{a:1}", headers);
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/SetMetadataTest/1");
                base.AssertNotNullGetResponse(response);

                var responseHeaders = response.RawResponse.Headers.GetValues("Some-Raven-Metadata-Key").GetEnumerator();
                if(responseHeaders.MoveNext())
                {
                    Assert.Equal("some-raven-metadata-value", responseHeaders.Current);
                }
            });
        }
    }
}
