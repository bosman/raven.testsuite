namespace Raven.TestSuite.Tests.Common.Attributes
{
    using System;

    using Xunit;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RavenDotNetApiTestAttribute : FactAttribute
    {
    }
}
