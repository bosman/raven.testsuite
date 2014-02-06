using Raven.TestSuite.Common.Abstractions;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequiresFreshNorthwindDatabaseAttribute : Attribute
    {
        public Action<ITestUnitEnvironment> DeployNorthwind()
        {
            return env =>
                {
                DeleteNorthwind();

                var runnigDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dumpPath =  Path.Combine(runnigDirectoryPath, Constants.Paths.NorthwindDumpPath);
                var args = env.SmugglerArgsBuilder()
                                      .ImportTo(env.DefaultDbAddress)
                                      .UsingDatabase(Constants.DbName.Northwind)
                                      .UsingFile(dumpPath).Build();
                env.RunSmuggler(args);

                MakeSureThereAreNoStaleIndexes(env, Constants.DbName.Northwind);
            };
        }

        public Action<ITestUnitEnvironment> DeleteNorthwind()
        {
            return DeleteNorthwind;
        }

        private static void DeleteNorthwind(ITestUnitEnvironment env)
        {
            using (var docStore = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
            {
                docStore.DeleteDatabase(Constants.DbName.Northwind);
            }
        }

        private static void MakeSureThereAreNoStaleIndexes(ITestUnitEnvironment env, string dbName)
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
