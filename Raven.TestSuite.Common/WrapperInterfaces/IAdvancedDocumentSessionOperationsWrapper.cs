using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IAdvancedDocumentSessionOperationsWrapper
    {
        string GetDocumentId(object entity);

        bool HasChanges { get; }

        int MaxNumberOfRequestsPerSession { get; set; }

        int NumberOfRequests { get; }

        string StoreIdentifier { get; }

        bool UseOptimisticConcurrency { get; set; }

        void Clear();

        void Evict<T>(T entity);

        EtagWrapper GetEtagFor<T>(T instance);

        RavenJObjectWrapper GetMetadataFor<T>(T instance);

        void SetMetadataValueFor<T>(T instance, string key, string value);

        bool HasChanged(object entity);

        bool IsLoaded(string id);

        void MarkReadOnly(object entity);
    }
}
