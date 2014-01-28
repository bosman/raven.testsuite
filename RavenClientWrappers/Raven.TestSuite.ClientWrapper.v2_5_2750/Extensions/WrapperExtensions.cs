using Raven.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Data;

namespace Raven.TestSuite.ClientWrapper._2_5_2750.Extensions
{
    public static class WrapperExtensions
    {
         public static Etag Unwrap(this EtagWrapper wrapped)
         {
             return new Etag(wrapped.ToString());
         }
    }
}