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

        RestResponse RawPut(string url, string content);

        RestResponse RawPost(string url, string content);

        RestResponse RawDelete(string url);

        #endregion

        #region Tools

        void RunSmuggler(string arguments);

        #endregion

    }
}
