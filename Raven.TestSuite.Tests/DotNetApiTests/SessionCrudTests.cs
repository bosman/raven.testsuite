using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Raven.TestSuite.Tests.DatabaseObjects;
using Xunit;

namespace Raven.TestSuite.Tests.DotNetApiTests
{
    public class SessionCrudTests : BaseTestGroup
    {

        [RavenDotNetApiTest]
        public void EntityAddedToANewDatabaseIsReturnedByIdFromTheLoadMethodInASeparateSession()
        {
            wrapper.Execute(testEnv =>
            {
                var initialPerson = new Person { Id = 1, FirstName = "Clark", LastName = "Kent" };

                using (var docStore = testEnv.CreateDocumentStore("TestDatabase").Initialize())
                {
                    using (var session = docStore.OpenSession())
                    {
                        session.Store(initialPerson);
                        session.SaveChanges();
                    }

                    using (var session = docStore.OpenSession())
                    {
                        var person = session.Load<Person>(1);
                        Assert.NotNull(person);
                        Assert.Equal(initialPerson.FirstName, person.FirstName);
                        Assert.Equal(initialPerson.LastName, person.LastName);
                    }

                    docStore.DeleteDatabase("TestDatabase");
                }
            });
        }
    }
}
