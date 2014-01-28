using System;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IJsonDocumentWrapper
    {
        RavenJObjectWrapper DataAsJson { get; set; }
        RavenJObjectWrapper Metadata { get; set; }
        string Key { get; set; }
        bool? NonAuthoritativeInformation { get; set; }
        EtagWrapper Etag { get; set; }
        DateTime? LastModified { get; set; }
        RavenJObjectWrapper ToJson();
    }
}