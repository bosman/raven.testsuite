using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IRavenClientWrapper
    {
        string GetRavenDllVersion();

        string GetWrapperVersion();

        void Execute(Action<ITestUnitEnvironment> action);

        void Execute(Action<IRavenClientWrapper> action);
    }
}
