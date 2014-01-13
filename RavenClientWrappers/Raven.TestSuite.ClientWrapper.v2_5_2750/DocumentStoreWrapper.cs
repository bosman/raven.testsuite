using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Document;
using Raven.TestSuite.ClientWrapper.v2_5_2750;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.ClientWrapper._2_5_2750
{
    public class DocumentStoreWrapper : IDocumentStoreWrapper
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
    }
}
