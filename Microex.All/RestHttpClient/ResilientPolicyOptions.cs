using System;
using System.Collections.Generic;
using System.Text;

namespace Microex.All.RestHttpClient
{
    public class ResilientPolicyOptions
    {
        /// <summary>
        /// 触发熔断的连续计数时间,默认10秒
        /// </summary>
        public int CircuitBreakerDuration { get; set; } = 10;
        /// <summary>
        /// 触发熔断的单位时间异常计数,默认5次
        /// </summary>
        public int CircuitBreakerExceptionCount { get; set; } = 5;

        /// <summary>
        /// 访问失败后的重试次数,默认3
        /// </summary>
        public int RetryCount { get; set; } = 3;
    }
}
