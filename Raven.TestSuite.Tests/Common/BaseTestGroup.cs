namespace Raven.TestSuite.Tests.Common
{
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
    }
}
