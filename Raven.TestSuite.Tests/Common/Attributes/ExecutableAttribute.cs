using Raven.TestSuite.Common.WrapperInterfaces;
using System;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [Serializable]
    public abstract class ExecutableAttribute : Attribute
    {
        public abstract Action<ITestUnitEnvironment> GetExecutableAction();
    }
}
