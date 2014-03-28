namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.Client;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.Json.Linq;
    using Raven.Imports.Newtonsoft.Json;
    using Raven.TestSuite.Common.Abstractions.Data;

    public class SyncAdvancedSessionOperationWrapper : ISyncAdvancedSessionOperationWrapper
    {
        private readonly ISyncAdvancedSessionOperation syncAdvancedSessionOperation;

        internal SyncAdvancedSessionOperationWrapper(ISyncAdvancedSessionOperation syncAdvancedSessionOperation)
        {
            this.syncAdvancedSessionOperation = syncAdvancedSessionOperation;
        }

        public IEagerSessionOperationsWrapper Eagerly
        {
            get { return new EagerSessionOperationsWrapper(this.syncAdvancedSessionOperation.Eagerly); }
        }

        public ILazySessionOperationsWrapper Lazily
        {
            get { return new LazySessionOperationsWrapper(this.syncAdvancedSessionOperation.Lazily); }
        }

        public T[] LoadStartingWith<T>(string keyPrefix, string matches = null, int start = 0, int pageSize = 25, string exclude = null)
        {
            return this.syncAdvancedSessionOperation.LoadStartingWith<T>(keyPrefix, matches, start, pageSize, exclude);
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

        public EtagWrapper GetEtagFor<T>(T instance)
        {
            return new EtagWrapper(this.syncAdvancedSessionOperation.GetEtagFor<T>(instance).ToString());
        }

        public RavenJObjectWrapper GetMetadataFor<T>(T instance)
        {
            return RavenJObjectWrapper.Parse(this.syncAdvancedSessionOperation.GetMetadataFor<T>(instance).ToString());
        }

        public void SetMetadataValueFor<T>(T instance, string key, string value)
        {
            this.syncAdvancedSessionOperation.GetMetadataFor<T>(instance)[key] = value;
        }

        public bool HasChanged(object entity)
        {
            return this.syncAdvancedSessionOperation.HasChanged(entity);
        }

        public bool IsLoaded(string id)
        {
            return this.syncAdvancedSessionOperation.IsLoaded(id);
        }

        public void MarkReadOnly(object entity)
        {
            this.syncAdvancedSessionOperation.MarkReadOnly(entity);
        }
    }
}
