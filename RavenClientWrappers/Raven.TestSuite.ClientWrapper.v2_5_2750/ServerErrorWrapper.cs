using Raven.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class ServerErrorWrapper : IServerErrorWrapper
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

        public static ServerErrorWrapper FromServerError(ServerError se)
        {
            if (se != null)
            {
                var result = new ServerErrorWrapper
                    {
                        Index = se.Index,
                        Error = se.Error,
                        Timestamp = se.Timestamp,
                        Document = se.Document,
                        Action = se.Action
                    };
                return result;
            }
            return null;
        }
    }
}
