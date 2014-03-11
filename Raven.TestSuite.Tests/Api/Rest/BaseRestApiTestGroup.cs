using System;

namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
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
    }
}
