using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IOperationWrapper
    {
        Task<RavenJTokenWrapper> WaitForCompletionAsync();
        RavenJTokenWrapper WaitForCompletion();
    }
}