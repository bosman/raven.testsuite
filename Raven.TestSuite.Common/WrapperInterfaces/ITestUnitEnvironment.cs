using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ITestUnitEnvironment
    {
        IDocumentStoreWrapper CreateDocumentStore(string defaultDatabase);
    }
}
