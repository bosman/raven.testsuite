using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.TestSuite.Common.Abstractions.Data;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDocumentStoreWrapper : IDisposable
    {
        //TODO: changes

        IDisposable AggressivelyCacheFor(TimeSpan cacheDuration);

        IDisposable AggressivelyCache();

        IDisposable DisableAggressiveCaching();

        IDisposable SetRequestsTimeoutFor(TimeSpan timeout);

        IDocumentSessionWrapper OpenSession();

        IDocumentSessionWrapper OpenSession(string database);

        IDatabaseCommandsWrapper DatabaseCommands { get; }

        //TODO execute index and transformer
        //TODO: conventions
        //TODO: bulk insert

        string Url { get; }

        IDocumentStoreWrapper Initialize();

        EtagWrapper GetLastWrittenEtag();

        void DeleteDatabase(string name);
    }
}
