using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class SuggestionOptionsWrapper : ISuggestionOptionsWrapper
    {
        public StringDistanceTypesWrapper Distance { get; set; }

        public float Accuracy { get; set; }

        protected bool Equals(SuggestionOptionsWrapper other)
        {
            return Distance == other.Distance && Accuracy.Equals(other.Accuracy);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SuggestionOptionsWrapper)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Distance * 397) ^ Accuracy.GetHashCode();
            }
        }
    }
}
