using Raven.TestSuite.Common.Abstractions;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects;
using System.Linq;
using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
using Xunit;

namespace Raven.TestSuite.Tests
{
    [RequiresFreshNorthwindDatabase]
    public class InitialDevTests : BaseTestGroup
    {
        public InitialDevTests(IRavenClientWrapper wrapper) : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        [PreinitializeDatabase("World")]
        public void SimpleTest1()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore("World").Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) > 0);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        [PreinitializeDatabase("World")]
        public void SimpleFailingTest1()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore("World").Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        Assert.True(session.Query<Country>().Count(o => o.Area > 1000000) < 0);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void CreationDeletion()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore("TestDatabase").Initialize())
                {
                    docStore.DeleteDatabase("TestDatabase");
                }
            });
        }

        [RavenDotNetApiTest]
        public void SomeNorthwindQuery()
        {
            wrapper.Execute(testEnv =>
            {
                using (var docStore = testEnv.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        var test = session.Query<Company>().Count();
                        var test2 = session.Query<Employee>().ToList();
                        Assert.True(session.Query<Company>().Count() == 91);
                    }
                }
            });
        }
    }
}
