using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common
{
    public interface IRavenClientWrapper
    {
        string GetVersion();

        IList<string> GetSomeStrings();

        K QueryInSession<T, K>(Func<IQueryable<T>, K> expression);

        T DoInSession<T>(Func<IDocumentSessionWrapper, T> expression);
    }
}
