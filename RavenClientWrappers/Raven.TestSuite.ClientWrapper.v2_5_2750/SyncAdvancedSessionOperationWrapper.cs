namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.Client;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.Json.Linq;
    using Raven.Imports.Newtonsoft.Json;

    public class SyncAdvancedSessionOperationWrapper : ISyncAdvancedSessionOperationWrapper
    {
        private readonly ISyncAdvancedSessionOperation syncAdvancedSessionOperation;

        internal SyncAdvancedSessionOperationWrapper(ISyncAdvancedSessionOperation syncAdvancedSessionOperation)
        {
            this.syncAdvancedSessionOperation = syncAdvancedSessionOperation;
        }

        public string GetDocumentUrl(object entity)
        {
            return this.syncAdvancedSessionOperation.GetDocumentUrl(entity);
        }

        public string GetDocumentId(object entity)
        {
            return this.syncAdvancedSessionOperation.GetDocumentId(entity);
        }

        public bool HasChanges 
        {
            get { return this.syncAdvancedSessionOperation.HasChanges; }
        }

        public int MaxNumberOfRequestsPerSession 
        {
            get { return this.syncAdvancedSessionOperation.MaxNumberOfRequestsPerSession; }
            set { this.syncAdvancedSessionOperation.MaxNumberOfRequestsPerSession = value; }
        }

        public int NumberOfRequests
        {
            get { return this.syncAdvancedSessionOperation.NumberOfRequests; }
        }

        public string StoreIdentifier 
        {
            get { return this.syncAdvancedSessionOperation.StoreIdentifier; }
        }

        public void Refresh<T>(T entity)
        {
            this.syncAdvancedSessionOperation.Refresh<T>(entity);
        }

        public bool UseOptimisticConcurrency
        {
            get { return this.syncAdvancedSessionOperation.UseOptimisticConcurrency; }
            set { this.syncAdvancedSessionOperation.UseOptimisticConcurrency = value; } 
        }

        public void Clear()
        {
            this.syncAdvancedSessionOperation.Clear();
        }

        public void Evict<T>(T entity)
        {
            this.syncAdvancedSessionOperation.Evict<T>(entity);
        }

        public RavenJObjectWrapper GetMetadataFor<T>(T instance)
        {
            return RavenJObjectWrapper.Parse(this.syncAdvancedSessionOperation.GetMetadataFor<T>(instance).ToString());
        }
    }
}
