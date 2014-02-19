using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ITestUnitEnvironment
    {
        IDocumentStoreWrapper CreateDocumentStore(string defaultDatabase);

        int DbPort { get; }

        string DefaultDbAddress { get; }

        #region REST HTTP Api

        RestResponse RawGet(string url);

        RestResponse RawPatch(string url, string content);

        RestResponse RawPut(string url, string content, Dictionary<string, List<string>> headers = null);

        RestResponse RawPost(string url, string content);

        RestResponse RawDelete(string url);

        #endregion

        #region Tools

        void RunSmuggler(string arguments);

        ISmugglerArgumentsBuilder SmugglerArgsBuilder();

        #endregion
    }
}
