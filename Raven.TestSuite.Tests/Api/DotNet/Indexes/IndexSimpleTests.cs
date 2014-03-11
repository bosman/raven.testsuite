using Raven.TestSuite.ClientWrapper.v2_5_2750;
using Raven.TestSuite.Common.Abstractions;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Raven.TestSuite.Tests.Api.DotNet.Indexes
{
    [RequiresFreshNorthwindDatabase]
    public class IndexSimpleTests : BaseTestGroup
    {
        public IndexSimpleTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void PutAndGetIndex()
        {
            wrapper.Execute(testEnv =>
                {
                    const string indexName = "PutAndGetIndexTestIndex";

                    using (var docStore = testEnv.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                    {
                        docStore.DatabaseCommands.PutIndex(indexName,
                                                           new IndexDefinitionWrapper
                                                               {
                                                                   Map = "docs.Products.Select(x => new { Name = x.Name })",
                                                                   InternalFieldsMapping = new Dictionary<string, string>()
                                                               });

                        var result = docStore.DatabaseCommands.GetIndex(indexName);
                        Assert.NotNull(result);
                        Assert.Equal(indexName, result.Name);
                    }
                });
        }

        [RavenDotNetApiTest]
        public void PutDeleteAndGetIndex()
        {
            wrapper.Execute(testEnv =>
            {
                const string indexName = "PutDeleteAndGetIndexTestIndex";

                using (var docStore = testEnv.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    docStore.DatabaseCommands.PutIndex(indexName,
                                                       new IndexDefinitionWrapper
                                                       {
                                                           Map = "docs.Products.Select(x => new { Name = x.Name })",
                                                           InternalFieldsMapping = new Dictionary<string, string>()
                                                       });

                    var result = docStore.DatabaseCommands.GetIndex(indexName);
                    Assert.NotNull(result);
                    Assert.Equal(indexName, result.Name);
                    docStore.DatabaseCommands.DeleteIndex(indexName);
                    var result2 = docStore.DatabaseCommands.GetIndex(indexName);
                    Assert.Null(result2);
                }
            });
        }

        [RavenDotNetApiTest]
        public void PutAndUseIndexInQuery()
        {
            wrapper.Execute(testEnv =>
            {
                const string indexName = "PutAndUseIndexInQueryTestIndex";

                using (var docStore = testEnv.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    docStore.DatabaseCommands.PutIndex(indexName,
                                                       new IndexDefinitionWrapper
                                                       {
                                                           Map = "docs.Products.Select(x => new { Name = x.Name })",
                                                           InternalFieldsMapping = new Dictionary<string, string>()
                                                       });

                    using (var session = docStore.OpenSession())
                    {
                        var results = session.Query<Product>(indexName).ToList();
                        Assert.True(results.Count > 0);
                    }
                }
            });
        }
    }
}
