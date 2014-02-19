namespace Raven.TestSuite.Tests.Api.Rest.Crud
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Xunit;

    [RequiresFreshNorthwindDatabaseAttribute]
    public class PatchTests : BaseRestApiTestGroup
    {
        public PatchTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenRestApiTest]
        public void CreateNewDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/1", "[{ Type: 'Set', Name: 'CreateNewDocumentTestProperty', Value: 'CreateNewDocumentTestValue'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/1");
                this.AssertNotNullGetResponse(response);
                Assert.Equal("CreateNewDocumentTestValue", response.RavenJTokenWrapper.Value<string>("CreateNewDocumentTestProperty"));
            });
        }

        [RavenRestApiTest]
        public void AddNewAttributeToDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/2",
                    "[{ Type: 'Set', Name: 'AddNewAttributeToDocumentTestAttribute', Value: 'AddNewAttributeToDocumentTestValue'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/2");
                this.AssertNotNullGetResponse(response);
                Assert.Equal("AddNewAttributeToDocumentTestValue", response.RavenJTokenWrapper.Value<string>("AddNewAttributeToDocumentTestAttribute"));
            });
        }

        [RavenRestApiTest]
        public void OverwriteAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/3",
                    "[{ Type: 'Set', Name: 'Name', Value: 'OverwriteAttributeTestValue'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/3");
                this.AssertNotNullGetResponse(response);
                Assert.Equal("OverwriteAttributeTestValue", response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }
    }
}
