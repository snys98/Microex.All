using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Microex.AngularSpa.AliyunSls
{
    public class AliyunSlsLoggingProvider:ILoggerProvider
    {

        public AliyunSlsLoggingProvider(AliyunSlsOptions aliyunSlsOptions,LogLevel minLevel)
        {
            AliyunSlsLogger.Config(minLevel,aliyunSlsOptions.RemoteEndPoint, aliyunSlsOptions.AccessKeyId, aliyunSlsOptions.AccessKeySecret, aliyunSlsOptions.Project, aliyunSlsOptions.LogStore);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new AliyunSlsLogger(categoryName);
        }
    }
}
