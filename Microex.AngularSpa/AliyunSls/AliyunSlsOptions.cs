using Microsoft.Extensions.Logging;

namespace Microex.AngularSpa.AliyunSls
{
    public class AliyunSlsOptions
    {
        public string RemoteEndPoint { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BulkName { get; set; }
        public string LogStore { get; set; }
        public string Project { get; set; }
        public LogLevel MinLevel { get; set; }
    }
}