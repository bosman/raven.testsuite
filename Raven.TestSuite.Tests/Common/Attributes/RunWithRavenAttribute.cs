using System;
using Raven.TestSuite.Tests.Common.Runner;
using Xunit;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RunWithRavenAttribute : RunWithAttribute
    {
        public RunWithRavenAttribute() : base(typeof(XunitRunner))
        {
        }
        
    }
}