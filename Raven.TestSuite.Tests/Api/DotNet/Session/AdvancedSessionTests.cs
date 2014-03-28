namespace Raven.TestSuite.Tests.Api.DotNet.Session
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
    using System;
    using System.IO;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class AdvancedSessionTests : BaseDotNetApiTestGroup
    {
        public AdvancedSessionTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void SetDocumentMetadataTest()
        {
            this.wrapper.Execute(env =>
            {
                const string cat1Id = "categories/1";
                const string attrKey = "SetDocumentMetadataTestKey";
                const string attrVal = "SetDocumentMetadataTestValue";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>(cat1Id);
                        Assert.NotNull(category);
                        session.Advanced.SetMetadataValueFor<Category>(category, attrKey, attrVal);
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>(cat1Id);
                        var result = session.Advanced.GetMetadataFor<Category>(category);
                        Assert.NotNull(result);
                        Assert.Equal(attrVal, result.Value<string>(attrKey));
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void GetDocumentUrlTest()
        {
            this.wrapper.Execute(env =>
            {
                const string cat2Id = "categories/2";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>(cat2Id);
                        Assert.NotNull(category);
                        var uri = new Uri(session.Advanced.GetDocumentUrl(category));
                        Assert.Equal(Constants.DbUrl.Northwind + "/docs/" + cat2Id, uri.AbsolutePath);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void RefreshDocumentTest()
        {
            this.wrapper.Execute(env =>
            {
                const string cat3Id = "categories/3";
                const string cat3Name = "RefreshDocumentTest";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>(cat3Id);
                        Assert.NotNull(category);
                        Assert.Equal("Confections", category.Name);

                        using (var innerSession = store.OpenSession())
                        {
                            var innerCategory = session.Load<Category>(cat3Id);
                            innerCategory.Name = cat3Name;
                            session.Store(innerCategory);
                            session.SaveChanges();
                        }

                        session.Advanced.Refresh<Category>(category);
                        Assert.Equal(cat3Name, category.Name);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void HasChangesTest()
        {
            this.wrapper.Execute(env =>
            {
                const string cat4Id = "categories/4";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>(cat4Id);
                        Assert.NotNull(category);
                        Assert.False(session.Advanced.HasChanges);
                        session.Store(category);
                        session.SaveChanges();
                        Assert.False(session.Advanced.HasChanges);

                        category.Name = "something";
                        Assert.True(session.Advanced.HasChanges);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void NumberOfRequestsTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Assert.Equal(0, session.Advanced.NumberOfRequests);

                        var category = new Category();
                        category.Name = "NumberOfRequestsTest";

                        session.Store(category);
                        session.SaveChanges();
                        Assert.Equal(1, session.Advanced.NumberOfRequests);

                        var category2 = session.Load<Category>(category.Id);
                        category2.Name = "NumberOfRequestsTest2";
                        session.Store(category2);
                        session.SaveChanges();
                        Assert.Equal(2, session.Advanced.NumberOfRequests);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void MaxNumberOfRequestsPerSessionTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        session.Advanced.MaxNumberOfRequestsPerSession = 2;

                        var category = new Category();
                        session.Store(category);
                        session.SaveChanges();
                        Assert.Equal(1, session.Advanced.NumberOfRequests);

                        category.Name = "1";
                        session.Store(category);
                        session.SaveChanges();
                        Assert.Equal(2, session.Advanced.NumberOfRequests);

                        try
                        {
                            category.Name = "2";
                            session.Store(category);
                            session.SaveChanges();
                            Assert.False(true, "I expected InvalidOperationException to be thrown here.");
                        }
                        catch (InvalidOperationException) 
                        {
                        }
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void StoreIdentifierTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Assert.Equal(Constants.DbName.Northwind, session.Advanced.StoreIdentifier.Split(';')[1]);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void OptimisticConcurrencyTest()
        {
            this.wrapper.Execute(env =>
            {
                const string cat5Id = "categories/5";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Assert.False(session.Advanced.UseOptimisticConcurrency);
                        session.Advanced.UseOptimisticConcurrency = true;

                        var category = session.Load<Category>(cat5Id);
                        category.Name = "someName";

                        using (var innerSession = store.OpenSession())
                        {
                            var innerCategory = session.Load<Category>(cat5Id);
                            category.Name = "someInnerName";
                            session.Store(category);
                            session.SaveChanges();
                        }

                        try
                        {
                            session.Store(category);
                            session.SaveChanges();
                            Assert.False(true, "I expected ConcurrencyException to be thrown here.");
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void ClearTest()
        {
            this.wrapper.Execute(env =>
            {
                const string employee1 = "Employees/1";
                const string employee1Name = "CrealTest";
                const string employee2 = "Employees/2";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var empl1 = session.Load<Employee>(employee1);
                        empl1.FirstName = employee1Name;
                        session.Store(empl1);

                        var empl2 = session.Load<Employee>(employee2);
                        session.Delete<Employee>(empl2);

                        session.Advanced.Clear();
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var empl1 = session.Load<Employee>(employee1);
                        Assert.NotEqual<string>(employee1Name, empl1.FirstName);

                        var empl2 = session.Load<Employee>(employee2);
                        Assert.NotNull(empl2);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void EvictTest()
        {
            this.wrapper.Execute(env =>
            {
                const string employee3 = "Employees/3";
                const string employee4 = "Employees/4";
                const string employee5 = "Employees/5";
                const string firstName = "EvictTest";
                
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var empl3 = session.Load<Employee>(employee3);
                        empl3.FirstName = firstName;
                        session.Store(empl3);

                        var empl4 = session.Load<Employee>(employee4);
                        session.Delete<Employee>(empl4);

                        var empl5 = session.Load<Employee>(employee5);
                        empl5.FirstName = firstName;
                        session.Store(empl5);

                        session.Advanced.Evict<Employee>(empl3);
                        session.Advanced.Evict<Employee>(empl4);
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var empl3 = session.Load<Employee>(employee3);
                        Assert.NotEqual<string>(firstName, empl3.FirstName);

                        var empl4 = session.Load<Employee>(employee4);
                        Assert.NotNull(empl4);

                        var empl5 = session.Load<Employee>(employee5);
                        Assert.Equal(firstName, empl5.FirstName);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void GetEtagForTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>("categories/6");
                        Assert.Equal("01000000-0000-0001-0000-0000000000A1", session.Advanced.GetEtagFor<Category>(category).ToString());
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void HasChangedTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var company = session.Load<Company>("companies/1");
                        Assert.NotNull(company);
                        Assert.False(session.Advanced.HasChanged(company));

                        company.Name = "HasChangedTest";
                        Assert.True(session.Advanced.HasChanged(company));
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void IsLoadedTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Assert.False(session.Advanced.IsLoaded("categories/7"));
                        session.Load<Category>("categories/7");
                        Assert.True(session.Advanced.IsLoaded("categories/7"));
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void MarkReadOnlyTest()
        {
            this.wrapper.Execute(env => 
            {
                const string categoryName = "MarkReadOnlyTest";

                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>("categories/7");
                        session.Advanced.MarkReadOnly(category);
                        category.Name = categoryName;
                        Assert.True(session.Advanced.HasChanges);

                        session.Store(category);
                        session.SaveChanges();
                    }

                    using (var session = store.OpenSession())
                    {
                        var category = session.Load<Category>("categories/7");
                        Assert.True(session.Advanced.GetMetadataFor<Category>(category).Value<bool>("Raven-Read-Only"));
                    }
                }
            });
        }
    }
}
