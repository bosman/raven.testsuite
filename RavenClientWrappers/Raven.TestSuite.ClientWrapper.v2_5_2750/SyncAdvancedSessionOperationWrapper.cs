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

        public string GetDocumentId(object entity)
        {
            return this.syncAdvancedSessionOperation.GetDocumentId(entity);
        }

        public RavenJObjectWrapper GetMetadataFor<T>(T instance)
        {
            return RavenJObjectWrapper.Parse(this.syncAdvancedSessionOperation.GetMetadataFor<T>(instance).ToString());
        }
    }
}
