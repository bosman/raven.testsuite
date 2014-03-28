namespace Raven.TestSuite.Tests.Api.DotNet.Indexes
{
    using Raven.TestSuite.ClientWrapper.v2_5_2750;
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Indexing;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Xunit;

    [RequiresFreshNorthwindDatabase]
    public class IndexStatsTests : BaseTestGroup
    {
        public IndexStatsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        public void IndexingAttemptsTest()
        {
            this.wrapper.Execute(env =>
            {
                const string indexName = "IndexingAttemptsTest";

                using (var docStore = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    docStore.DatabaseCommands.PutIndex(indexName,
                        new IndexDefinitionWrapper
                        {
                            Map = "from product in docs.Products where product.Discontinued == true select new { product.Discontinued }"
                        });

                    var result = docStore.DatabaseCommands.GetIndex(indexName);
                    Assert.NotNull(result);
                    Assert.Equal(indexName, result.Name);
                }
            });
        }
    }
}
