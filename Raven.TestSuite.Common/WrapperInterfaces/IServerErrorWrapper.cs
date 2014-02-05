using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IServerErrorWrapper
    {
        string Index { get; set; }
        string Error { get; set; }
        DateTime Timestamp { get; set; }
        string Document { get; set; }
        string Action { get; set; }
    }
}
