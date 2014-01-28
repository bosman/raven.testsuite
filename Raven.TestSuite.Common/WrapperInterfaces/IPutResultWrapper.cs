using Raven.TestSuite.Common.Abstractions.Data;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IPutResultWrapper
    {
        string Key { get; set; }
        EtagWrapper ETag { get; set; }
    }
}