using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.Abstractions.Data
{
    [Flags]
    public enum IndexingPriorityWrapper
    {
        None = 0,

        Normal = 1,

        Disabled = 2,

        Idle = 4,

        Abandoned = 8,

        Forced = 512,
    }
}
