namespace Raven.TestSuite.Common.Data
{
    using Raven.TestSuite.Common.WrapperInterfaces;
    using System;

    public class ServerErrorWrapper
    {
        public string Index { get; set; }
        public string Error { get; set; }
        public DateTime Timestamp { get; set; }
        public string Document { get; set; }
        public string Action { get; set; }

        public override string ToString()
        {
            return string.Format("Index: {0}, Error: {1}, Document: {2}, Action: {3}", Index, Error, Document, Action);
        }
    }
}
