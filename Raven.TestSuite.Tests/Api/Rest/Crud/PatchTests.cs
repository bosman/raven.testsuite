using System;

namespace Raven.TestSuite.Tests.Api.Rest.Crud
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Xunit;

    [Serializable]
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
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/1",
                    "[{ Type: 'Set', Name: 'CreateNewDocumentTestProperty', Value: 'CreateNewDocumentTestValue'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/1");
                base.AssertNotNullGetResponse(response);
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
                base.AssertNotNullGetResponse(response);
                Assert.Equal("AddNewAttributeToDocumentTestValue", response.RavenJTokenWrapper.Value<string>("AddNewAttributeToDocumentTestAttribute"));
            });
        }

        [RavenRestApiTest]
        public void OverrideAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/3",
                    "[{ Type: 'Set', Name: 'Name', Value: 'OverwriteAttributeTestValue'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/3");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("OverwriteAttributeTestValue", response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }

        [RavenRestApiTest]
        public void OverrideAttributeWithMatchingPrevValTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/4",
                    "[{ Type: 'Set', Name: 'Name', Value: 'OverrideAttributeWithCorrectPrevValueTestValue', PrevVal: 'Dairy Products'}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/4");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("OverrideAttributeWithCorrectPrevValueTestValue", response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }

        [RavenRestApiTest]
        public void OverrideAttributeWithNotMatchingPrevValTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/categories/5",
                    "[{ Type: 'Set', Name: 'Name', Value: 'OverrideAttributeWithIncorrectPrevValueTestValue', PrevVal: 'aaa'}]");
                Assert.Equal(409, (int)response.RawResponse.StatusCode);
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/categories/5");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("Grains/Cereals", response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }

        [RavenRestApiTest]
        public void SetNullValueTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/1",
                    "[{ Type: 'Set', Name: 'Freight', Value: null}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/1");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<float?>("Freight"));
            });
        }

        [RavenRestApiTest]
        public void OverrideAttributeSetNullValueWithMatchingPrevValIsAlwaysSuccessTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/2",
                    "[{ Type: 'Set', Name: 'Freight', Value: null, PrevVal: null}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/2");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<float?>("Freight"));
            });
        }

        [RavenRestApiTest]
        public void SetArrayValueTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/3",
                    "[{ Type: 'Set', Name: 'ShipVia', Value: [{'field1': '1', 'field2': 2}]}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/3");
                base.AssertNotNullGetResponse(response);

                //TODO compare these lists of RavenJToken
            });
        }
    }
}
