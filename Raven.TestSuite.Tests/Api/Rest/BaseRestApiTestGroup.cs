using System;

namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common;
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
    using System.Threading;
    using Xunit;

    [Serializable]
    public class BaseRestApiTestGroup : BaseTestGroup
    {
        protected BaseRestApiTestGroup(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        protected void AssertNotNullGetResponse(RestResponse response)
        {
            Assert.Equal(200, (int)response.RawResponse.StatusCode);
            Assert.NotNull(response.RavenJTokenWrapper);
        }

        protected void WaitForIndexes(int wait = 20)
        {
            this.wrapper.Execute(env =>
            {
                DateTime stop = DateTime.Now.AddSeconds(wait);
                while (DateTime.Compare(DateTime.Now, stop) < 0)
                {
                    var response = env.RawGet(Constants.DbUrl.Northwind + "/stats");
                    if (response.RavenJTokenWrapper.Value<RavenJArrayWrapper>("StaleIndexes").Length == 0)
                    {
                        return;
                    }
                    Thread.Sleep(100);
                }
                throw new Exception("There are still stale indexes after " + wait + " seconds.");
            });
        }
    }
}
