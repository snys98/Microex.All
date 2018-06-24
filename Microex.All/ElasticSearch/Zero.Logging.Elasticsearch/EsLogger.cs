using System;
using System.Collections.Generic;
using Microex.All.ElasticSearch.Zero.Logging.Commom;
using Microsoft.Extensions.Logging;

namespace Microex.All.ElasticSearch.Zero.Logging.Elasticsearch
{
    public class EsLogger : ILogger
    {
        private readonly BatchingLoggerProvider _provider;
        private readonly string _category;
        private readonly string _serviceName;
        private readonly string _serverIp;
        private readonly string _env;

        public EsLogger(BatchingLoggerProvider loggerProvider, string categoryName, string env, string serviceName,
            string serverIp)
        {
            _provider = loggerProvider;
            _category = categoryName;
            _serviceName = serviceName;
            _serverIp = serverIp;
            _env = env;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsEnabled;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.Now, logLevel, eventId, state, exception, formatter);
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var jsonData = new { timestamp = timestamp, level = logLevel.ToString(), env = _env, serviceName = _serviceName, serverIp = _serverIp, category = _category, eventId, message = formatter(state, exception), exceptions = new List<ExceptionModel>() };
            if (exception != null)
            {
                WriteSingleException(jsonData.exceptions, exception, 0);
            }
            _provider.AddMessage(timestamp, Newtonsoft.Json.JsonConvert.SerializeObject(jsonData));
        }

        private void WriteException(List<ExceptionModel> exceptionList, Exception exception, int depth)
        {
            WriteSingleException(exceptionList, exception, depth);
            if (exception.InnerException != null && depth < 20)
                WriteException(exceptionList, exception.InnerException, ++depth);
        }

        private void WriteSingleException(dynamic exceptionList, Exception exception, int depth)
        {
            exceptionList.Add(new ExceptionModel
            {
                depth = depth,
                message = exception.Message,
                source = exception.Source,
                stackTrace = exception.StackTrace,
                hResult = exception.HResult,
                helpLink = exception.HelpLink
            });
        }

        internal class ExceptionModel
        {
            public int depth { get; set; }
            public string message { get; set; }
            public string source { get; set; }
            public string stackTrace { get; set; }
            public int hResult { get; set; }
            public string helpLink { get; set; }
        }
    }
}
