namespace Raven.TestSuite.Tests.Api.DotNet.Session
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using System;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class AdvancedTests : BaseDotNetApiTestGroup
    {
        public AdvancedTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }
    }
}
