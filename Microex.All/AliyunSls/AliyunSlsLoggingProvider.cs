//using System;
//using Microsoft.Extensions.Logging;

//namespace Microex.All.AliyunSls
//{
//    public class AliyunSlsLoggingProvider:ILoggerProvider
//    {

//        public AliyunSlsLoggingProvider(AliyunSlsOptions aliyunSlsOptions,LogLevel autologMinLevel)
//        {
//            AliyunSlsLogger.Config(autologMinLevel,aliyunSlsOptions.RemoteEndPoint, aliyunSlsOptions.AccessKeyId, aliyunSlsOptions.AccessKeySecret, aliyunSlsOptions.Project, aliyunSlsOptions.LogStore);
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public ILogger CreateLogger(string categoryName)
//        {
//            return new AliyunSlsLogger(categoryName);
//        }
//    }
//}
