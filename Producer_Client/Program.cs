using System;
using RabbitMQ.Client;

namespace Producer_Client
{
    class Program
    {

        /// <summary>
        /// http://www.rabbitmq.com/dotnet-api-guide.html
        /// </summary>
        /// <param name="args"></param>
        //sender
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var factory = new ConnectionFactory();

            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            var exchangeName = "";
            var routingKey = "hello";
            //首先，需要创建一个ConnectionFactory
            using (var conn = factory.CreateConnection())
            {
                //紧接着要创建一个Channel，如果要发送消息，需要创建一个队列，然后将消息发布到这个队列中。
                //在创建队列的时候，只有RabbitMQ上该队列不存在，才会去创建。
                //消息是以二进制数组的形式传输的，所以如果消息是实体对象的话，需要序列化和然后转化为二进制数组。
                using (var channel = conn.CreateModel())
                {
                    //(1)保证队列和消息都是持久化的。
                    //queueDeclare 这个改动需要在发送端和接收端同时设置。
                    bool durable = true;
                    channel.QueueDeclare("hello", durable, false, false, null);

                    string message = "hello world1 w";
                    var body = System.Text.Encoding.UTF8.GetBytes(message);
                    //To publish a message to an exchange, use IModel.BasicPublish as follows:
                    // channel.BasicPublish(exchangeName, routingKey, null, body);

                    #region properties
                    //For fine control, you can use overloaded variants to specify the mandatory flag, or specify messages properties:
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.ContentType = "text/plain";
                    properties.DeliveryMode = 2;
                    //This sends a message with delivery mode 2(persistent) and content-type "text/plain".See the definition of the IBasicProperties interface for more information about the available message properties.
                    //In the following example, we publish a message with custom headers:
                    properties.Headers = new System.Collections.Generic.Dictionary<string, object>();
                    properties.Headers.Add("latitude", 51.5252949);
                    properties.Headers.Add("longitude", -0.0905493);
                    //Code sample below sets a message expiration
                    properties.Expiration = "36000";

                    //(2)保证消息也是持久化的
                    //需要注意的是，将消息设置为持久化并不能完全保证消息不丢失。虽然他告诉RabbitMQ将消息保存到磁盘上，但是在RabbitMQ接收到消息和将其保存到磁盘上这之间仍然有一个小的时间窗口。 RabbitMQ 可能只是将消息保存到了缓存中，并没有将其写入到磁盘上。持久化是不能够一定保证的，但是对于一个简单任务队列来说已经足够。如果需要消息队列持久化的强保证，可以使用publisher confirms
                    properties.Persistent = true;
                    #endregion
                     
                    channel.BasicPublish(exchangeName, routingKey, properties, body);
                     
                    Console.WriteLine("已发送：{0}", message);

                    while (true)
                    {
                        message = Console.ReadLine();
                        body = System.Text.Encoding.UTF8.GetBytes(message);
                        //channel.BasicPublish("", "hello", null, bodys);
                        channel.BasicPublish(exchangeName, routingKey, null, body);

                        Console.WriteLine("已发送：{0}", message);
                    }
                }
            }

        }
    }
}
