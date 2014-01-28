using Raven.TestSuite.Common.Abstractions.Enums;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ISuggestionOptionsWrapper
    {
        StringDistanceTypesWrapper Distance { get; set; }
        float Accuracy { get; set; }
    }
}