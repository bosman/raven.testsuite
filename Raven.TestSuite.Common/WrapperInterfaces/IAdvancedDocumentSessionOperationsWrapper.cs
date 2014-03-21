using Raven.TestSuite.Common.Abstractions.Json.Linq;
namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IAdvancedDocumentSessionOperationsWrapper
    {
        string GetDocumentId(object entity);

        RavenJObjectWrapper GetMetadataFor<T>(T instance);
    }
}
