using System;
using Xunit;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class RavenTestAttribute : FactAttribute
    {
        public string TestTypeName
        {
            get { return this.GetType().Name.Remove(this.GetType().Name.IndexOf("Attribute", StringComparison.InvariantCulture)); }
        }
    }
}
