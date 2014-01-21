using System;
using Raven.Client.Document;
using Raven.TestSuite.ClientWrapper.v2_5_2750;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.Client.Connection;

namespace Raven.TestSuite.ClientWrapper._2_5_2750
{
    public class DocumentStoreWrapper : IDocumentStoreWrapper, IDisposable
    {
        private DocumentStore documentStore;

        public DocumentStoreWrapper(DocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public IDocumentSessionWrapper OpenSession()
        {
            return new DocumentSessionWrapper(this.documentStore.OpenSession());
        }

        public IDocumentStoreWrapper Initialize()
        {
            this.documentStore.Initialize();
            return this;
        }

        public void Dispose()
        {
            this.documentStore.Dispose();
        }

        public IEtagWrapper GetLastWrittenEtag()
        {
            return new EtagWrapper(this.documentStore.GetLastWrittenEtag());
        }

        public void DeleteDatabase(string name)
        {
            var commands = documentStore.DatabaseCommands.ForSystemDatabase();
            var url = "/admin/databases/" + name + "?hard-delete=true";
            ((ServerClient)commands).CreateRequest("DELETE", url).ExecuteRequest();
        }
    }
}
