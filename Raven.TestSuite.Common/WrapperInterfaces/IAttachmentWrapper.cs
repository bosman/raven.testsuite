using System;
using System.IO;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IAttachmentWrapper
    {
        Func<Stream> Data { get; set; }

        int Size { get; set; }

        RavenJObjectWrapper Metadata { get; set; }

        EtagWrapper Etag { get; set; }

        string Key { get; set; }
    }
}