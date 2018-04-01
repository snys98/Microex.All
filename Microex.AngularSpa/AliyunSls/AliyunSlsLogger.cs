using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Aliyun.Api.LOG;
using Aliyun.Api.LOG.Data;
using Aliyun.Api.LOG.Request;
using Aliyun.Api.LOG.Response;
using Aliyun.Api.SLS.Response;
using Microex.AngularSpa.Extensions;
using Microsoft.Extensions.Logging;

namespace Microex.AngularSpa.AliyunSls
{
    public class AliyunSlsLogger:ILogger
    {
        private string _catagory;
        private static string _endpoint;
        private static string _accesskeyId;
        private static string _accessKey;
        private static string _project;
        private static string _logstore;
        private static LogLevel _minLevel;

        public AliyunSlsLogger(string catagory)
        {
            _catagory = catagory;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogClient client = new LogClient(_endpoint, _accesskeyId, _accessKey);
            //init http connection timeout
            client.ConnectionTimeout = client.ReadWriteTimeout = 10000;
            //put logs
            PutLogsRequest logsRequest = new PutLogsRequest();
            logsRequest.Project = _project;
            logsRequest.Topic = _catagory;
            logsRequest.Logstore = _logstore;
            logsRequest.LogItems = new List<LogItem>
            {
                new LogItem()
                {
                    Contents = new List<LogContent>()
                    {
                        new LogContent()
                        {
                            Key = eventId.ToJson(true),
                            Value = new {state, exception}.ToJson(true)
                        }
                    }
                }
            };
            PutLogsResponse putLogRespError = client.PutLogs(logsRequest);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _minLevel>=logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public static void Config(LogLevel minLevel, string remoteEndPoint, string accessKeyId, string accessKeySecret, string project, string logStore)
        {
            _endpoint = remoteEndPoint;
            _accesskeyId = accessKeyId;
            _accessKey = accessKeySecret;
            _project = project;
            _logstore = logStore;
        }
    }
}
