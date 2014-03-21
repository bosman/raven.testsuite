namespace Raven.TestSuite.Tests.Api.DotNet.Documents
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class LoadDocumentTests : BaseDotNetApiTestGroup
    {
        public LoadDocumentTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void GetNotExistingDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var response = session.Load<Product>("NonExisting/1");
                        Assert.Null(response);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void GetExistingDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var response = session.Load<Category>("categories/1");
                        Assert.NotNull(response);
                        Assert.Equal("Beverages", response.Name);
                    }
                }
            });
        }
    }
}
