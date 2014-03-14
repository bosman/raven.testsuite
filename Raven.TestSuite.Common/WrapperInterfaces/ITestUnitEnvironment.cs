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

        RestResponse RawDelete(string url, string query = null);

        RestResponse RawGet(string url, string query = null);

        RestResponse RawHead(string url);

        RestResponse RawPatch(string url, string content, string query = null, Dictionary<string, List<string>> headers = null);

        RestResponse RawPut(string url, string content, Dictionary<string, List<string>> headers = null);

        RestResponse RawPost(string url, string content, string query = null);


        #endregion

        #region Tools

        void RunSmuggler(string arguments);

        ISmugglerArgumentsBuilder SmugglerArgsBuilder();

        #endregion
    }
}
