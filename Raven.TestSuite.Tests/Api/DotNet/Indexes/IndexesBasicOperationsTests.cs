namespace Raven.TestSuite.Tests.Api.DotNet.Indexes
{
    using Raven.TestSuite.ClientWrapper.v2_5_2750;
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabase]
    public class IndexesBasicOperationsTests : BaseDotNetApiTestGroup
    {
        public IndexesBasicOperationsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void CreateAndDeleteSimpleIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                const string indexName = "CreateAndDeleteSimpleIndexTest";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    store.DatabaseCommands.PutIndex(indexName, 
                        new IndexDefinitionWrapper {
                            Map = "docs.Products.Select(x => new { Name = x.Name })",
                            InternalFieldsMapping = new Dictionary<string, string>()
                        });
                    base.WaitForStaleIndexes(env, Constants.DbName.Northwind);

                    var result = store.DatabaseCommands.GetIndex(indexName);
                    Assert.NotNull(result);
                    Assert.Equal(indexName, result.Name);

                    store.DatabaseCommands.DeleteIndex(indexName);
                    result = store.DatabaseCommands.GetIndex(indexName);
                    Assert.Null(result);
                }
            });
        }

        [RavenDotNetApiTest]
        public void CreateAndQuerySimpleMapIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                const string indexName = "CreateAndQuerySimpleMapIndexTest";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    store.DatabaseCommands.PutIndex(indexName,
                        new IndexDefinitionWrapper
                        {
                            Map = "from product in docs.Products select new { product.Discontinued }",
                            InternalFieldsMapping = new Dictionary<string, string>()
                        });
                    base.WaitForStaleIndexes(env, Constants.DbName.Northwind);

                    using (var session = store.OpenSession())
                    {
                        var result = session.Query<Product>(indexName)
                            .Where(x => x.Discontinued.Equals(true))
                            .ToArray<Product>();
                        Assert.NotNull(result);
                        Assert.Equal(8, result.Count());
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void CreateAndQuerySimpleMapReduceIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                const string indexName = "CreateAndQuerySimpleMapReduceIndexTest";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    store.DatabaseCommands.PutIndex(indexName, 
                        new IndexDefinitionWrapper {
                            Map = "from order in docs.Orders select new { order.Company, order.Freight }",
                            Reduce = "from result in results group result by result.Company into g select new { Company = g.Key, Freight = g.Sum(x=>x.Freight) }",
                            InternalFieldsMapping = new Dictionary<string, string>()
                        });
                    base.WaitForStaleIndexes(env, Constants.DbName.Northwind);

                    using (var session = store.OpenSession())
                    {
                        var result = session.Query<Order>(indexName).ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(89, result.Count());

                        result = session.Query<Order>(indexName)
                            .Where(x => x.Company.Equals("companies/52"))
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(322.04m, result.First().Freight);

                        result = session.Query<Order>(indexName)
                            .Where(x => x.Company.Equals("companies/24"))
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(1678.08m, result.First().Freight);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void CreateAndQuerySimpleIndexWithPagingTest()
        {
            const string indexName = "CreateAndQuerySimpleIndexWithPagingTest";

            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    store.DatabaseCommands.PutIndex(indexName,
                        new IndexDefinitionWrapper
                        {
                            Map = "from product in docs.Products select new { product.Discontinued }",
                            InternalFieldsMapping = new Dictionary<string, string>()
                        });
                    base.WaitForStaleIndexes(env, Constants.DbName.Northwind);

                    using (var session = store.OpenSession())
                    {
                        var result = session.Query<Product>(indexName)
                            .Where(x => x.Discontinued.Equals(true))
                            .Take(2)
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(2, result.Count());
                        Assert.Equal("products/17", result[0].Id);
                        Assert.Equal("products/24", result[1].Id);

                        result = session.Query<Product>(indexName)
                            .Where(x => x.Discontinued.Equals(true))
                            .Skip(2)
                            .Take(2)
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(2, result.Count());
                        Assert.Equal("products/28", result[0].Id);
                        Assert.Equal("products/29", result[1].Id);

                        result = session.Query<Product>(indexName)
                            .Where(x => x.Discontinued.Equals(true))
                            .Skip(4)
                            .Take(5)
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(4, result.Count());
                        Assert.Equal("products/42", result[0].Id);
                        Assert.Equal("products/5", result[1].Id);
                        Assert.Equal("products/53", result[2].Id);
                        Assert.Equal("products/9", result[3].Id);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void SimpleQueryDynamicIndexTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        session.Query<Product>()
                            .Where(x => x.Discontinued.Equals(true));
                        base.WaitForStaleIndexes(env, Constants.DbName.Northwind);

                        var result = session.Query<Product>()
                            .Where(x => x.Discontinued.Equals(true))
                            .ToArray();
                        Assert.NotNull(result);
                        Assert.Equal(8, result.Count());
                    }
                }
            });
        }
    }
}
