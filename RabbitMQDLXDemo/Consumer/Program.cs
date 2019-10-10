using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "127.0.0.1", UserName = "oec2003", Password = "123456" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                    channel.QueueDeclare("queue-2", false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body); 
                        Console.WriteLine("已接收： {0}", message);   
                    };
                    channel.BasicConsume("queue-2", false, consumer);
            }
            Console.ReadLine(); 
        }
    }
}
