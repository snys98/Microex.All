using System;

namespace Microex.All.ElasticSearch.Zero.Logging.Commom
{
    public struct LogMessage
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Message { get; set; }
    }
}
