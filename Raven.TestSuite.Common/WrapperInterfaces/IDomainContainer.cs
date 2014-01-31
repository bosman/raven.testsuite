using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDomainContainer : IDisposable
    {
        IRavenClientWrapper Wrapper { get; }
    }
}
