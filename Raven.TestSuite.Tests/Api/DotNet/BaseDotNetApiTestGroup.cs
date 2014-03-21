using System;

namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
    using System.Linq;
    using System.Threading;

    [Serializable]
    public class BaseDotNetApiTestGroup : BaseTestGroup
    {
        protected BaseDotNetApiTestGroup(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        protected void WaitForStaleIndexes(ITestUnitEnvironment env, string dbName)
        {
            using (var docStore = env.CreateDocumentStore(dbName).Initialize())
            {
                while (true)
                {
                    var stats = docStore.DatabaseCommands.GetStatistics();
                    if (stats != null && !stats.StaleIndexes.Any())
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
            }
        }
    }
}
