namespace Raven.TestSuite.Tests.Api.DotNet.Documents
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class StoreDocumentTests : BaseDotNetApiTestGroup
    {
        public StoreDocumentTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void StoringDocumentWithTheSameIdInDifferentSessionShouldOverrideTheDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = new Category();
                        category.Id = "ReplaceDocumentTest";
                        category.Name = "ReplaceDocumentTestName";

                        session.Store(category);
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var category = new Category();
                        category.Id = "ReplaceDocumentTest";
                        category.Name = "ReplaceDocumentTestName2";

                        session.Store(category);
                        session.SaveChanges();

                        var result = session.Load<Category>("ReplaceDocumentTest");
                        Assert.NotNull(result);
                        Assert.Equal("ReplaceDocumentTestName2", result.Name);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void StoringDocumentWithTheSameIdInTheSameSessionShouldResultWithExceptionTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = new Category();
                        category.Id = "ReplaceDocumentTest";
                        category.Name = "ReplaceDocumentTestName";

                        session.Store(category);
                        session.SaveChanges();

                        category = new Category();
                        category.Id = "ReplaceDocumentTest";
                        category.Name = "ReplaceDocumentTestName2";

                        try
                        {
                            session.Store(category);
                            session.SaveChanges();
                            Assert.True(false, "I expected NonUniqueObjectException to be thrown here.");
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            });
        }
    }
}
