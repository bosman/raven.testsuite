﻿namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using System.Collections.Generic;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class AttachmentOperationsTests : BaseRestApiTestGroup
    {
        public AttachmentOperationsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void PutGetAndDeleteAttachmentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/static/documents/test.txt",
                    "text PutGetAndDeleteAttachmentTest");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("/static/documents/test.txt", response.RawResponse.Headers.Location.ToString());

                response = env.RawPut(Constants.DbUrl.Northwind + "/static/documents/test.txt",
                    "text2 PutGetAndDeleteAttachmentTest");
                Assert.Equal(201, (int)response.RawResponse.StatusCode);
                Assert.Equal("/static/documents/test.txt", response.RawResponse.Headers.Location.ToString());

                response = env.RawGet(Constants.DbUrl.Northwind + "/static/documents/test.txt");
                Assert.Equal(200, (int)response.RawResponse.StatusCode);
                Assert.Equal("text2 PutGetAndDeleteAttachmentTest", response.RawResponse.Content.ReadAsStringAsync().Result);

                var headers = new Dictionary<string, List<string>>();
                headers.Add("Author", new List<string>(new string[] { "some guy" }));
                response = env.RawPost(Constants.DbUrl.Northwind + "/static/documents/test.txt",
                    "", "", headers);
                Assert.Equal(200, (int)response.RawResponse.StatusCode);

                response = env.RawHead(Constants.DbUrl.Northwind + "/static/documents/test.txt");
                Assert.Equal(200, (int)response.RawResponse.StatusCode);
                Assert.NotNull(response.RawResponse.Headers.GetValues("Author"));
                var enumerator = response.RawResponse.Headers.GetValues("Author").GetEnumerator();
                enumerator.MoveNext();
                Assert.Equal("some guy", enumerator.Current.ToString());

                response = env.RawDelete(Constants.DbUrl.Northwind + "/static/documents/test.txt");
                Assert.Equal(204, (int)response.RawResponse.StatusCode);
                response = env.RawGet(Constants.DbUrl.Northwind + "/static/documents/test.txt");
                Assert.Equal(404, (int)response.RawResponse.StatusCode);
                Assert.Null(response.RavenJTokenWrapper);
            });
        }
    }
}
