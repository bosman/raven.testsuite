using Raven.TestSuite.Common.Abstractions.Enums;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface ISpatialOptionsWrapper
    {
        SpatialFieldTypeWrapper Type { get; set; }
        SpatialSearchStrategyWrapper Strategy { get; set; }
        int MaxTreeLevel { get; set; }
        double MinX { get; set; }
        double MaxX { get; set; }
        double MinY { get; set; }
        double MaxY { get; set; }
        SpatialUnitsWrapper Units { get; set; }
    }
}