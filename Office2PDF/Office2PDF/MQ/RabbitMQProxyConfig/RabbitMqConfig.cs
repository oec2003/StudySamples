using System;

namespace Office2PDF.MQ
{
    /// <summary>
    /// 
    /// </summary>
    public class MqConfig
    {
        public string Host { get; set; }
        public ushort HeartBeat { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public TimeSpan NetworkRecoveryInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Port { get; set; }

    }

    /// <summary>
    /// 死信队列实体
    /// </summary>
    [RabbitMq("dead-letter-{Queue}", ExchangeName = "dead-letter-{exchange}")]
    public class DeadLetterQueue
    {
        public string Body { get; set; }

        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }

        public int RetryCount { get; set; }

        public string ExceptionMsg { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
