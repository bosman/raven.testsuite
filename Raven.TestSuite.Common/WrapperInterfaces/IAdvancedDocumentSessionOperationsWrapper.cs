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

        RavenJObjectWrapper GetMetadataFor<T>(T instance);
    }
}
