using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common
{
    public interface IDocumentSessionWrapper : IDisposable
    {
        IOrderedQueryable<T> Query<T>();
    }
}
