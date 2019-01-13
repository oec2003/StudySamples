using Office2PDF.Messages;
using Office2PDF.MQ;
using System;
using System.Threading;
using System.Configuration;
using Office2PDF.Common;

namespace Office2PDF
{
    class Program
    {
        static IPowerPointConverter converter = new PowerPointConverter();
        static void Main(string[] args)
        {
            var mqManager = new MQManager(new MqConfig
            {
                AutomaticRecoveryEnabled = true,
                HeartBeat = 60,
                NetworkRecoveryInterval = new TimeSpan(60),

                Host = ConfigurationManager.AppSettings["mqhostname"], 
                UserName = ConfigurationManager.AppSettings["mqusername"],
                Password = ConfigurationManager.AppSettings["mqpassword"],
                Port = ConfigurationManager.AppSettings["mqport"]
            });

            if (mqManager != null && mqManager.Connected)
            {
                Console.WriteLine("RabbitMQ连接初始化成功。");
                Console.WriteLine("RabbitMQ消息接收中...");

                mqManager.Subscribe<PowerPointConvertMessage>(message =>
                {
                    if (message != null)
                    {
                        converter.OnWork(message);
                        Console.WriteLine(message.FileInfo);
                    }
                });
            }
            else
            {
                Console.WriteLine("RabbitMQ连接初始化失败,请检查连接。");
                Console.ReadLine();
            }
        }
    }
}
