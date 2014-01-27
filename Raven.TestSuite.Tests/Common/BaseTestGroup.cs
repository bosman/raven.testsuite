namespace Raven.TestSuite.Tests.Common
{
    using System.Threading.Tasks;

    using Raven.TestSuite.Common.WrapperInterfaces;

    public abstract class BaseTestGroup
    {
        protected IRavenClientWrapper wrapper;

        protected BaseTestGroup()
        {
            wrapper = Raven.TestSuite.ClientWrapper.v2_5_2750.Wrapper.Create();
        }

        protected BaseTestGroup(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        protected async Task DeployNorthwindAsync()
        {
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
