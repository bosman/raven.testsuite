using System;
using Raven.Client.Document;
using Raven.TestSuite.ClientWrapper.v2_5_2750;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.Client.Connection;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DocumentStoreWrapper : IDocumentStoreWrapper, IDisposable
    {
        private readonly DocumentStore documentStore;

        public DocumentStoreWrapper(DocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public IDisposable AggressivelyCacheFor(TimeSpan cacheDuration)
        {
            return documentStore.AggressivelyCacheFor(cacheDuration);
        }

        public IDisposable AggressivelyCache()
        {
            return documentStore.AggressivelyCache();
        }

        public IDisposable DisableAggressiveCaching()
        {
            return documentStore.DisableAggressiveCaching();
        }

        public IDisposable SetRequestsTimeoutFor(TimeSpan timeout)
        {
            return documentStore.SetRequestsTimeoutFor(timeout);
        }

        public IDocumentSessionWrapper OpenSession()
        {
            return new DocumentSessionWrapper(this.documentStore.OpenSession());
        }

        public IDocumentSessionWrapper OpenSession(string database)
        {
            return new DocumentSessionWrapper(documentStore.OpenSession(database));
        }

        public IDatabaseCommandsWrapper DatabaseCommands { get { return new DatabaseCommandsWrapper(documentStore.DatabaseCommands); } }
        public string Url { get { return documentStore.Url; } }

        public IDocumentStoreWrapper Initialize()
        {
            this.documentStore.Initialize();
            return this;
        }

        public void Dispose()
        {
            this.documentStore.Dispose();
        }

        public EtagWrapper GetLastWrittenEtag()
        {
            var etag = this.documentStore.GetLastWrittenEtag();
            if (etag != null)
            {
                return new EtagWrapper(etag);
            }
            return null;
        }

        public void DeleteDatabase(string name)
        {
            var commands = documentStore.DatabaseCommands.ForSystemDatabase();
            var url = "/admin/databases/" + name + "?hard-delete=true";
            ((ServerClient)commands).CreateRequest("DELETE", url).ExecuteRequest();
        }
    }
}
