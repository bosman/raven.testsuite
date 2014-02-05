using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class AttachmentWrapper : IAttachmentWrapper
    {
        public Func<Stream> Data { get; set; }

        public int Size { get; set; }

        public RavenJObjectWrapper Metadata { get; set; }

        public EtagWrapper Etag { get; set; }

        public string Key { get; set; }
    }
}
