using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDocumentSessionWrapper : IDisposable
    {
        //TODO: finish me
        IOrderedQueryable<T> Query<T>();

        void Store(dynamic entity);

        T Load<T>(ValueType id);

        void SaveChanges();
    }
}
