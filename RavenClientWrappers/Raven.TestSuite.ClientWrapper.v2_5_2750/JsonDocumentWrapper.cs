using System;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper._2_5_2750
{
    public class JsonDocumentWrapper : IJsonDocumentWrapper
    {
        private readonly JsonDocument inner;
        public JsonDocumentWrapper(JsonDocument document)
        {
            inner = document;
        }

        //TODO: change impl to use inner object
        public RavenJObjectWrapper DataAsJson { get; set; }
        public RavenJObjectWrapper Metadata { get; set; }
        public string Key { get; set; }
        public bool? NonAuthoritativeInformation { get; set; }
        public EtagWrapper Etag { get; set; }
        public DateTime? LastModified { get; set; }
        public RavenJObjectWrapper ToJson()
        {
            throw new NotImplementedException();
        }
    }
}