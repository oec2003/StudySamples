using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1", UserName = "oec2003", Password = "123456" };
            using (var connection = factory.CreateConnection())
            while (Console.ReadLine() != null)
            {
                using (var channel = connection.CreateModel())
                {
                    var arguments = new Dictionary<string, object>();
                    arguments.Add("x-dead-letter-exchange", "exchange-2");
                    arguments.Add("x-dead-letter-routing-key", "rk-2");

                    channel.QueueDeclare("queue-1",true,false,false,arguments);
                
                    channel.ExchangeDeclare("exchange-2", "direct");
                    channel.QueueDeclare("queue-2", false, false, false, null);
                    channel.QueueBind("queue-2", "exchange-2", "rk-2", null);

                    var message = "Hello oec2003!";
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Expiration = "5000";

                    channel.BasicPublish("", "queue-1", properties, body);
                    Console.WriteLine($"发送： {message}");
                }
            }
            Console.ReadKey();
        }
    }
}
