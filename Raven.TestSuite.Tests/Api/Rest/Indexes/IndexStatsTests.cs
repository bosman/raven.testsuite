namespace Raven.TestSuite.Tests.Api.Rest.Indexes
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class IndexStatsTests : BaseRestApiTestGroup
    {
        public IndexStatsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }
    }
}
