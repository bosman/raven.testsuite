namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabase]
    public class LuceneQuerySyntaxTests : BaseRestApiTestGroup
    {
        public LuceneQuerySyntaxTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }


        [RavenRestApiTest]
        public void QueryDynamicIndexUsingOrOperatorTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Name:(Chang OR Ikura)");
                Assert.Equal(2, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(item.Value<string>("Name") == "Chang" || item.Value<string>("Name") == "Ikura");
                }

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Name:(Chang OR \"Antonio Moreno Taquería\")");
                Assert.Equal(2, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(item.Value<string>("Name") == "Chang" || item.Value<string>("Name") == "Antonio Moreno Taquería");
                }

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Name:Chang OR FirstName:Nancy");
                Assert.Equal(2, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(item.Value<string>("Name") == "Chang" || item.Value<string>("FirstName") == "Nancy");
                }
            });
        }

        [RavenRestApiTest]
        public void QueryDynamicIndexUsingAndOperatorTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=FirstName:Nancy and LastName:Davolio");
                Assert.Equal(1, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(item.Value<string>("FirstName") == "Nancy" || item.Value<string>("LastName") == "Davolio");
                }
            });
        }

        [RavenRestApiTest]
        public void QueryDynamicIndexUsingWildcardSearchTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:companies/?4");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:companies/?4");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(80, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(System.Text.RegularExpressions.Regex.IsMatch(item.Value<string>("Company"), "companies/.4"));
                }

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:co*ies/34");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:co*ies/34");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(14, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(System.Text.RegularExpressions.Regex.IsMatch(item.Value<string>("Company"), "co.*ies/34"));
                }
            });
        }

        [RavenRestApiTest]
        public void QueryDynamicIndexUsingRangeSearchTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:[companies/30 TO companies/32]");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:[companies/30 TO companies/32]");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(30, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.True(item.Value<string>("Company").Equals("companies/30") ||
                        item.Value<string>("Company").Equals("companies/31") ||
                        item.Value<string>("Company").Equals("companies/32"));
                }

                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:{companies/30 TO companies/32}");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Company:{companies/30 TO companies/32}");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(9, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.Equal("companies/31", item.Value<string>("Company"));
                }
            });
        }

        [RavenRestApiTest]
        public void QueryDynamicIndexUsingNestedPropertiesTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=ShipTo.City:Reims");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=ShipTo.City:Reims");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(5, response.RavenJTokenWrapper.Value<int>("TotalResults"));
                var result = response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("Results");
                foreach (RavenJTokenWrapper item in result)
                {
                    Assert.Equal("Reims", item.SelectToken("ShipTo").Value<string>("City"));
                }
            });
        }

        [RavenRestApiTest]
        public void QueryDynamicIndexUsingArraySearchTest()
        {
            this.wrapper.Execute(env =>
            {
                var response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Lines,Product:products/2");
                base.WaitForIndexes();
                response = env.RawGet(Constants.DbUrl.Northwind + "/indexes/dynamic", "query=Lines,Product:products/2");
                base.AssertNotNullGetResponse(response);
                Assert.Equal(44, response.RavenJTokenWrapper.Value<int>("TotalResults"));
            });
        }
    }
}
