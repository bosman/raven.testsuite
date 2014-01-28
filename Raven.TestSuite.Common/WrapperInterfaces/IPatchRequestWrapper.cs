using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IPatchRequestWrapper
    {
        PatchCommandTypeWrapper Type { get; set; }
        RavenJTokenWrapper PrevVal { get; set; }
        RavenJTokenWrapper Value { get; set; }
        IPatchRequestWrapper[] Nested { get; set; }
        string Name { get; set; }
        int? Position { get; set; }
        bool? AllPositions { get; set; }

        RavenJObjectWrapper ToJson();
    }
}