using System.Collections.Generic;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IScriptedPatchRequestWrapper
    {
        string Script { get; set; }
        Dictionary<string, object> Values { get; set; }
    }
}