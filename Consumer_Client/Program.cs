using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer_Client
{
    //Receive
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var factory = new ConnectionFactory();
            //factory.HostName = "localhost";
            //factory.UserName = "guest";
            //factory.Password = "guest";
            //factory.AutomaticRecoveryEnabled = false;
            using (var conn = factory.CreateConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume("hello", false, consumer);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"已接收：{message}");
                        channel.BasicAck(ea.DeliveryTag, false); 
                    };
                    
                    Console.ReadLine();
                }
            }
        }
    }
}
