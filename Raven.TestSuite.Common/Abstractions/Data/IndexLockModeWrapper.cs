using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.Abstractions.Data
{
    public enum IndexLockModeWrapper
    {
        Unlock,
        LockedIgnore,
        LockedError
    }
}
