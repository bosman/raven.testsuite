using Raven.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.ClientWrapper.v2_5_2750.Extensions;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class PutResultWrapper : IPutResultWrapper
    {
        private readonly PutResult inner;

        public PutResultWrapper(PutResult inner)
        {
            this.inner = inner;
        }

        public string Key
        {
            get { return inner.Key; }
            set { inner.Key = value; }
        }

        public EtagWrapper ETag
        {
            get { return new EtagWrapper(inner.ETag); }
            set { inner.ETag = value.Unwrap(); }
        }
    }
}