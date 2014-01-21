using System;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PreinitializeDatabaseAttribute : ExecutableAttribute
    {
        public string DatabaseName { get; private set; }

        public PreinitializeDatabaseAttribute(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public override Action<ITestUnitEnvironment> GetExecutableAction()
        {
            return testEnv =>
                {
                    using (var docStore = testEnv.CreateDocumentStore(this.DatabaseName).Initialize())
                    {
                        using (var session = docStore.OpenSession())
                        {
                            var etag = docStore.GetLastWrittenEtag();
                        }
                    }
                };
        }
    }
}
