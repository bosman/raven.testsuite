namespace Raven.TestSuite.Tests.Api.Rest
{
    using Raven.TestSuite.Common;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common;
    using Xunit;

    public class BaseRestApiTestGroup : BaseTestGroup
    {
        protected BaseRestApiTestGroup(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        protected void AssertNotNullGetResponse(RestResponse response)
        {
            Assert.NotNull(response.RavenJTokenWrapper);
            Assert.Equal((int)response.RawResponse.StatusCode, 200);
        }
    }
}
