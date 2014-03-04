using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class SpatialOptionsWrapper : ISpatialOptionsWrapper
    {
        public SpatialFieldTypeWrapper Type { get; set; }

        public SpatialSearchStrategyWrapper Strategy { get; set; }

        public int MaxTreeLevel { get; set; }

        public double MinX { get; set; }

        public double MaxX { get; set; }

        public double MinY { get; set; }

        public double MaxY { get; set; }

        public SpatialUnitsWrapper Units { get; set; }

        protected bool Equals(SpatialOptionsWrapper other)
        {
            var result = Type == other.Type && Strategy == other.Strategy;

            if (Type == SpatialFieldTypeWrapper.Geography)
            {
                result = result && Units == other.Units;
            }

            if (Strategy != SpatialSearchStrategyWrapper.BoundingBox)
            {
                result = result && MaxTreeLevel == other.MaxTreeLevel;

                if (Type == SpatialFieldTypeWrapper.Cartesian)
                {
                    result = result
                        && MinX.Equals(other.MinX)
                        && MaxX.Equals(other.MaxX)
                        && MinY.Equals(other.MinY)
                        && MaxY.Equals(other.MaxY);
                }
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpatialOptionsWrapper)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (int)Strategy;

                if (Type == SpatialFieldTypeWrapper.Geography)
                {
                    hashCode = (hashCode * 397) ^ Units.GetHashCode();
                }

                if (Strategy != SpatialSearchStrategyWrapper.BoundingBox)
                {
                    hashCode = (hashCode * 397) ^ MaxTreeLevel;

                    if (Type == SpatialFieldTypeWrapper.Cartesian)
                    {
                        hashCode = (hashCode * 397) ^ MinX.GetHashCode();
                        hashCode = (hashCode * 397) ^ MaxX.GetHashCode();
                        hashCode = (hashCode * 397) ^ MinY.GetHashCode();
                        hashCode = (hashCode * 397) ^ MaxY.GetHashCode();
                    }
                }

                return hashCode;
            }
        }
    }
}
