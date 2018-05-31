namespace Microex.All.AliyunOss
{
    public class AliyunOssOptions
    {
        public string LocalEndPoint { get; set; } = "fileupload";
        public string RemoteEndPoint { get; set; } = "oss-cn-shanghai.aliyuncs.com";
        public string ImageUrlPrefix { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string BulkName { get; set; }
    }
}