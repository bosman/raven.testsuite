using System;

namespace Raven.TestSuite.Tests.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RavenSmugglerTestAttribute : RavenTestAttribute
    {
    }
}
