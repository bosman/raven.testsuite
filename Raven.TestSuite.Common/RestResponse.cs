using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common
{
    public class RestResponse
    {
        public HttpResponseMessage RawResponse { get; set; }

        public RavenJTokenWrapper RavenJTokenWrapper { get; set; }
    }
}
