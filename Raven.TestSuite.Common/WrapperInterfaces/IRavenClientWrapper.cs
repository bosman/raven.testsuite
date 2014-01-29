using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IRavenClientWrapper
    {
        string GetVersion();

        void Execute(Action<ITestUnitEnvironment> action);

        void Execute(Action<IRavenClientWrapper> action);

        void Execute(Action<IRestEnvironment> action);
    }
}
