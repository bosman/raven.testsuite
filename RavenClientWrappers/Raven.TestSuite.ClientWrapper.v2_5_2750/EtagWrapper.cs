using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper._2_5_2750
{
    public class EtagWrapper : IEtagWrapper
    {
        private Etag etag;

        public EtagWrapper(Etag etag)
        {
            this.etag = etag;
        }

        public override string ToString()
        {
            return etag.ToString();
        }
    }
}
