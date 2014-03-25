namespace Raven.TestSuite.Common.WrapperInterfaces
{

    public interface ISyncAdvancedSessionOperationWrapper : IAdvancedDocumentSessionOperationsWrapper
    {
        IEagerSessionOperationsWrapper Eagerly { get; }

        ILazySessionOperationsWrapper Lazily { get; }

        string GetDocumentUrl(object entity);

        void Refresh<T>(T entity);
    }
}
