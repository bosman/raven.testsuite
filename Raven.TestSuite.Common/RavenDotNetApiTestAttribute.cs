using System;
using Xunit;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RavenDotNetApiTestAttribute : FactAttribute
    {
    }
}
