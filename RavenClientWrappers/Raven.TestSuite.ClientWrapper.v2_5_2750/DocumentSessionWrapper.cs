using Raven.Client;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.Linq;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DocumentSessionWrapper : MarshalByRefObject, IDocumentSessionWrapper
    {
        private IDocumentSession documentSession;

        internal DocumentSessionWrapper(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void Delete<T>(T entity)
        {
            this.documentSession.Delete<T>(entity);
        }

        public ISyncAdvancedSessionOperationWrapper Advanced
        {
            get { return new SyncAdvancedSessionOperationWrapper(this.documentSession.Advanced); }
        }

        public IOrderedQueryable<T> Query<T>()
        {
            return this.documentSession.Query<T>();
        }

        public IOrderedQueryable<T> Query<T>(string indexName, bool isMapReduce = false)
        {
            return this.documentSession.Query<T>(indexName, isMapReduce);
        }

        public void Dispose()
        {
            if (this.documentSession != null)
            {
                this.documentSession.Dispose();
            }
        }

        public void Store(dynamic entity)
        {
            this.documentSession.Store(entity);
        }

        public void SaveChanges()
        {
            this.documentSession.SaveChanges();
        }

        public T Load<T>(string id)
        {
            return this.documentSession.Load<T>(id);
        }

        public T Load<T>(ValueType id)
        {
            return this.documentSession.Load<T>(id);
        }
    }
}
