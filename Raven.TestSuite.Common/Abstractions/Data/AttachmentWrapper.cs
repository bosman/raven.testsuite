namespace Raven.TestSuite.Common.Data
{
    using Raven.TestSuite.Common.Abstractions.Data;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using System;
    using System.IO;

    public class AttachmentWrapper
    {
        public Func<Stream> Data { get; set; }

        public int Size { get; set; }

        public RavenJObjectWrapper Metadata { get; set; }

        public EtagWrapper Etag { get; set; }

        public string Key { get; set; }
    }
}
