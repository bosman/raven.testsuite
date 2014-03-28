namespace Raven.TestSuite.Common.Data
{
    using Raven.TestSuite.Common.Abstractions.Data;

    public class PutResultWrapper
    {
        public string Key { get; set; }
        public EtagWrapper ETag { get;  set; }
    }
}