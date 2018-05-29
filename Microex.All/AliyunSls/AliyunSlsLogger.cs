//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Aliyun.LOG;
//using Aliyun.LOG.Common.Utilities;
//using Aliyun.LOG.Data;
//using Aliyun.LOG.Request;
//using Aliyun.LOG.Response;
//using IdentityServer4.Extensions;
//using Microex.All.Extensions;
//using Microsoft.Extensions.Logging;

//namespace Microex.All.AliyunSls
//{
//    public class AliyunSlsLogger:ILogger
//    {
//        private string _catagory;
//        private static string _endpoint;
//        private static string _accesskeyId;
//        private static string _accessKey;
//        private static string _project;
//        private static string _logstore;
//        private static LogLevel _autologMinLevel;

//        public AliyunSlsLogger(string catagory)
//        {
//            _catagory = catagory;
//        }
//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//            if (!this.IsEnabled(logLevel) && formatter.Invoke(state, exception).IsNullOrEmpty())
//            {
//                return;
//            }

//            if (!eventId.Equals(default(EventId)))
//            {
//                Debug.WriteLine("eventId will be ignored", "warning");
//            }
//            try
//            {
//                LogClient client = new LogClient(_endpoint, _accesskeyId, _accessKey);
//                //init http connection timeout
//                client.ConnectionTimeout = client.ReadWriteTimeout = 10000;
//                //put logs
//                PutLogsRequest logsRequest = new PutLogsRequest();
//                logsRequest.Project = _project;
//                logsRequest.Topic = _catagory;
//                logsRequest.Logstore = _logstore;
//                logsRequest.LogItems = new List<LogItem>
//                {
//                    new LogItem()
//                    {
//                        Time = DateUtils.TimeSpan(),
//                        Contents = new List<LogContent>()
//                    }
//                };
//                if (!Equals(state, default(TState)))
//                {
//                    logsRequest.LogItems[0].Contents.Add(new LogContent()
//                    {
//                        Key = nameof(state),
//                        Value = state.ToJson(true)
//                    });
//                }

//                if (exception!=default(Exception))
//                {
//                    logsRequest.LogItems[0].Contents.Add(new LogContent()
//                    {
//                        Key = nameof(exception),
//                        Value = exception.ToJson(true)
//                    });
//                }

//                if (!Equals(formatter, default(Func<TState, Exception, string>)))
//                {
//                    logsRequest.LogItems[0].Contents.Add(new LogContent()
//                    {
//                        Key = "formatted",
//                        Value = formatter.Invoke(state,exception)
//                    });
//                }
                
//                PutLogsResponse putLogRespError = client.PutLogs(logsRequest);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                throw;
//            }
//        }

//        public bool IsEnabled(LogLevel logLevel)
//        {
//            return _autologMinLevel>=logLevel;
//        }

//        public IDisposable BeginScope<TState>(TState state)
//        {
//            return null;
//        }

//        public static void Config(LogLevel autologMinLevel, string remoteEndPoint, string accessKeyId, string accessKeySecret, string project, string logStore)
//        {
//            _endpoint = remoteEndPoint;
//            _accesskeyId = accessKeyId;
//            _accessKey = accessKeySecret;
//            _project = project;
//            _logstore = logStore;
//            _autologMinLevel = autologMinLevel;
//        }
//    }
//}
