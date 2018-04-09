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
                    bool durable = true;
                    channel.QueueDeclare("hello", durable, false, false, null);
                    #region 公平分发
                    //你可能会注意到，消息的分发可能并没有如我们想要的那样公平分配。比如，对于两个工作者。当奇数个消息的任务比较重，但是偶数个消息任务比较轻时，奇数个工作者始终处理忙碌状态，而偶数个工作者始终处理空闲状态。但是RabbitMQ并不知道这些，他仍然会平均依次的分发消息。
                    //了改变这一状态，我们可以使用basicQos方法，设置perfetchCount = 1 。这样就告诉RabbitMQ 不要在同一时间给一个工作者发送多于1个的消息，或者换句话说。在一个工作者还在处理消息，并且没有响应消息之前，不要给他分发新的消息。相反，将这条新的消息发送给下一个不那么忙碌的工作者。
                    //channel.BasicQos(0, 1, false);
                    #endregion

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
