using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDocumentSessionWrapper : IDisposable
    {
        IOrderedQueryable<T> Query<T>();
    }
}
