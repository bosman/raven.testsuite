using Xunit.Sdk;

namespace Raven.TestSuite.Common
{
    public interface IRavenCommand
    {
        MethodResult Execute(object testClass);
    }
}