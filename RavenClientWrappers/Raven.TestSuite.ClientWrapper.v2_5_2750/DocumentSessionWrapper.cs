using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DocumentSessionWrapper : MarshalByRefObject, IDocumentSessionWrapper
    {
        private IDocumentSession documentSession;

        internal DocumentSessionWrapper(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public IOrderedQueryable<T> Query<T>()
        {
            return this.documentSession.Query<T>();
        }

        public void Dispose()
        {
            if (this.documentSession != null)
            {
                this.documentSession.Dispose();
            }
        }


    }
}
