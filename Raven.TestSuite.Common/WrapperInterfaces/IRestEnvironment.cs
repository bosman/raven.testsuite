using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IRestEnvironment
    {
        HttpResponseMessage RawGet(string url);

        HttpResponseMessage RawPut(string url, string content);

        HttpResponseMessage RawPost(string url, string content);

        HttpResponseMessage RawDelete(string url);
    }
}
