namespace Raven.TestSuite.Common.WrapperInterfaces
{
    using System;
    using System.Linq;

    public interface IDocumentSessionWrapper : IDisposable
    {
        ISyncAdvancedSessionOperationWrapper Advanced { get; }

        //TODO: finish me
        IOrderedQueryable<T> Query<T>();

        IOrderedQueryable<T> Query<T>(string indexName, bool isMapReduce = false);

        void Store(dynamic entity);

        T Load<T>(string id);

        T Load<T>(ValueType id);

        void SaveChanges();
    }
}
