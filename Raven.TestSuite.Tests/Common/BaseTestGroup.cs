namespace Raven.TestSuite.Tests.Common
{
    using System.Threading.Tasks;
    using System;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using System.Threading;

    [Serializable]
    public abstract class BaseTestGroup
    {
        protected IRavenClientWrapper wrapper;

        protected BaseTestGroup()
        {
            throw new NotSupportedException("This way initializing test group is not supported.");
            wrapper = Raven.TestSuite.ClientWrapper.v2_5_2750.Wrapper.Create();
        }

        protected BaseTestGroup(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        protected async Task DeployNorthwindAsync()
        {
            throw new NotSupportedException("This way of deploying Northwind is not supported.");
            using (var northwind = typeof(BaseTestGroup).Assembly.GetManifestResourceStream("Raven.TestSuite.Tests.Assets.Northwind.dump"))
            {
                //var smugglerOptions = new SmugglerOptions
                //{
                //    OperateOnTypes = ItemType.Documents | ItemType.Indexes | ItemType.Transformers,
                //    ShouldExcludeExpired = false,
                //};

                //var smuggler = new SmugglerApi(smugglerOptions, DatabaseCommands, s => Report(s));

                //await smuggler.ImportData(northwind, smugglerOptions);
            }
        }

        protected void DeployNorthwind()
        {
            DeployNorthwindAsync().Wait();
        }
    }
}
