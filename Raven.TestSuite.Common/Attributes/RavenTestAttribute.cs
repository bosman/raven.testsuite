using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RavenTestAttribute : Attribute
    {
    }
}
