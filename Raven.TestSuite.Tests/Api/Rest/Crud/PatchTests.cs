namespace Raven.TestSuite.Tests.Api.Rest.Crud
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
                    "[{ Type: 'Set', Name: 'ShipVia', Value: [{'field1': '1', 'field2': 2}] }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/3");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(1, response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("ShipVia").Length);

                var values = new List<RavenJTokenWrapper>();
                values = (List<RavenJTokenWrapper>)response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("ShipVia").Values();
                Assert.Equal("1", values[0].Value<string>("field1"));
                Assert.Equal(2, values[0].Value<int>("field2"));
            });
        }

        [RavenRestApiTest]
        public void UnsetAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/4",
                    "[{ Type: 'Unset', Name: 'Name' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/4");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<string>("Name"));
            });
        }

        [RavenRestApiTest]
        public void IncrementByOneTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/products/1",
                    "[{ Type: 'Inc', Name: 'PricePerUser', Value: 1 }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/products/1");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(19, response.RavenJTokenWrapper.Value<int>("PricePerUser"));
            });
        }

        [RavenRestApiTest]
        public void DecrementByOneTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/products/2",
                    "[{ Type: 'Inc', Name: 'PricePerUser', Value: -1 }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/products/2");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(18, response.RavenJTokenWrapper.Value<int>("PricePerUser"));
            });
        }

        [RavenRestApiTest]
        public void IncrementByNonIntegerValueReturnsErrorTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/products/3",
                    "[{ Type: 'Inc', Name: 'PricePerUser', Value: 1.1 }]");
                Assert.Equal(500, (int)response.RawResponse.StatusCode);
            });
        }

        [RavenRestApiTest]
        public void CopyExistingAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/5",
                    "[{ Type: 'Copy', Name: 'Company', Value: 'CompanyCopy' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/5");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(response.RavenJTokenWrapper.Value<string>("Company"), response.RavenJTokenWrapper.Value<string>("CompanyCopy"));
                
                env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/5",
                    "[{ Type: 'Copy', Name: 'ShipTo', Value: 'ShipToCopy' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/5");
                Assert.True(
                    RavenJTokenWrapper.DeepEquals(
                        response.RavenJTokenWrapper.SelectToken(new RavenJPathWrapper("ShipTo")),
                        response.RavenJTokenWrapper.SelectToken(new RavenJPathWrapper("ShipToCopy"))
                        )
                    );
            });
        }

        [RavenRestApiTest]
        public void CopyNotExisitngAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/6",
                    "[{ Type: 'Copy', Name: 'Company1', Value: 'CompanyCopy' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/6");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<string>("CompanyCopy"));
            });
        }

        [RavenRestApiTest]
        public void RenameExistingAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/7",
                    "[{ Type: 'Rename', Name: 'Employee', Value: 'EmployeeCopy' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/7");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<string>("Employee"));
                Assert.Equal("employees/5", response.RavenJTokenWrapper.Value<string>("EmployeeCopy"));
            });
        }

        [RavenRestApiTest]
        public void RenameNotExisitngAttributeTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/8",
                    "[{ Type: 'Rename', Name: 'SomeNotExisitngAttribute', Value: 'ExistingAttribute' }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/8");
                base.AssertNotNullGetResponse(response);
                Assert.Null(response.RavenJTokenWrapper.Value<string>("ExisitngAttribute"));
            });
        }

        [RavenRestApiTest]
        public void ModifyArrayElementAtGivenPositionTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/9",
                    "[{ Type: 'Modify', Name: 'Lines', Position: 0, Nested: [{ Type: 'Set', Name: 'Quantity', Value: -1 }] }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/9");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(-1, response.RavenJTokenWrapper.SelectToken("Lines[0]").Value<int>("Quantity"));

                env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/9",
                    "[{ Type: 'Modify', Name: 'Lines[1]', Nested: [{ Type: 'Set', Name: 'Quantity', Value: -1 }] }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/9");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(-1, response.RavenJTokenWrapper.SelectToken("Lines[1]").Value<int>("Quantity"));
            });
        }

        [RavenRestApiTest]
        public void NestedInsideNestedTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPut(Constants.DbUrl.Northwind + "/docs/tests/1",
                    "{array: [{ nested: [{  name: 'testName' }] }]}");
                Assert.NotNull(response.RavenJTokenWrapper);
                Assert.NotNull(response.RavenJTokenWrapper.Value<string>("ETag"));
                Assert.Equal("tests/1", response.RavenJTokenWrapper.Value<string>("Key"));
                Assert.Equal(201, (int)response.RawResponse.StatusCode);

                env.RawPatch(Constants.DbUrl.Northwind + "/docs/tests/1",
                    "[{ Type: 'Modify', Name: 'array[0]', Nested: [{ Type: 'Modify', Name: 'nested[0]', Nested: [{ Type: 'Set', Name: 'name', Value: -1 }]}]}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/tests/1");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(-1, response.RavenJTokenWrapper.SelectToken("array[0].nested[0]").Value<int>("name"));
            });
        }

        [RavenRestApiTest]
        public void AddItemToNotExisitngArrayTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/9",
                    "[{ Type: 'Add', Name: 'testArray', Value: {testAttribute: 'testValue'}}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/9");
                base.AssertNotNullGetResponse(response);
                Assert.Equal("testValue", response.RavenJTokenWrapper.SelectToken("testArray[0]").Value<string>("testAttribute"));
            });
        }

        [RavenRestApiTest]
        public void AddItemToExisitngArray()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/10",
                    "[{ Type: 'Add', Name: 'Lines', Value: {testAttribute: 'testValue'}}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/10");
                base.AssertNotNullGetResponse(response);

                var lines = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Lines");
                Assert.Equal("testValue", lines[lines.Length - 1].Value<string>("testAttribute"));
            });
        }

        [RavenRestApiTest]
        public void InsertIntoArrayAtGivenPositionTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/11",
                    "[{ Type: 'Add', Name: 'Lines', Position: 0, Value: {testAttribute: 'testValue'}}]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/11");
                base.AssertNotNullGetResponse(response);

                var lines = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Lines");
                Assert.Equal("testValue", lines[0].Value<string>("testAttribute"));

                env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/11",
                    "[{ Type: 'Add', Name: 'Lines', Position: 1, Value: {testAttribute: 'testValue'}}]");
                base.AssertNotNullGetResponse(response);

                lines = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Lines");
                Assert.Equal("testValue", lines[1].Value<string>("testAttribute"));
            });
        }

        [RavenRestApiTest]
        public void RemoveFromArrayTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/12",
                    "[{ Type: 'Remove', Name: 'Lines', Position: 1 }]");
                response = env.RawGet(Constants.DbUrl.Northwind + "/docs/orders/12");
                base.AssertNotNullGetResponse(response);

                var lines = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Lines");
                Assert.Equal(1, lines.Length);
                Assert.Equal("products/21", lines[0].Value<string>("Product"));

                response = env.RawPatch(Constants.DbUrl.Northwind + "/docs/orders/13",
                    "[{ Type: 'Remove', Name: 'Lines', Position: 100 }]");
                Assert.Equal(500, (int)response.RawResponse.StatusCode);
            });
        }
    }
}
