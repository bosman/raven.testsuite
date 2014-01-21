using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.Tests.Common
{
    public class BaseTestGroup
    {
        protected IRavenClientWrapper wrapper;

        public BaseTestGroup(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }
    }
}
