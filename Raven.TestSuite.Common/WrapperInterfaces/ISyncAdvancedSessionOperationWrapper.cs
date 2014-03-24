namespace Raven.TestSuite.Common.WrapperInterfaces
{

    public interface ISyncAdvancedSessionOperationWrapper : IAdvancedDocumentSessionOperationsWrapper
    {
        string GetDocumentUrl(object entity);

        void Refresh<T>(T entity);
    }
}
