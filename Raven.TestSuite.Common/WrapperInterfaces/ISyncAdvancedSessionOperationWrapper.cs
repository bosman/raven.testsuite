namespace Raven.TestSuite.Common.WrapperInterfaces
{

    public interface ISyncAdvancedSessionOperationWrapper : IAdvancedDocumentSessionOperationsWrapper
    {
        IEagerSessionOperationsWrapper Eagerly { get; }

        ILazySessionOperationsWrapper Lazily { get; }

        T[] LoadStartingWith<T>(string keyPrefix, string matches = null, int start = 0, int pageSize = 25, string exclude = null);

        string GetDocumentUrl(object entity);

        void Refresh<T>(T entity);
    }
}
