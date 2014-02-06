using System;

namespace Raven.TestSuite.Common.Exceptions
{
    public class RavenVersionNotSupportedException : ApplicationException
    {
        public RavenVersionNotSupportedException(string version)
            : base(string.Format("Raven version {0} is not supported.", version))
        {

        }
    }
}
