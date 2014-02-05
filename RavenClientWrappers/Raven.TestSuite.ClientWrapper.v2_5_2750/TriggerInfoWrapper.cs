using Raven.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class TriggerInfoWrapper : ITriggerInfoWrapper
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public static TriggerInfoWrapper FromTriggerInfo(DatabaseStatistics.TriggerInfo ti)
        {
            if (ti != null)
            {
                var result = new TriggerInfoWrapper
                    {
                        Type = ti.Type,
                        Name = ti.Name
                    };
                return result;
            }
            return null;
        }
    }
}
