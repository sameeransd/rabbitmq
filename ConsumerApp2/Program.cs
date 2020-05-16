using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDomain;
using System;
using System.ComponentModel.DataAnnotations;

namespace ConsumerApp2
{
    class Program
    {
        private static readonly string hostname = "localhost";
        private static readonly string username = "guest";
        private static readonly string password = "guest";

        static void Main(string[] args)
        {
            Console.WriteLine("Program Starting...");
            ReceiveDirectRoutingQueue();
        }


        static void ReceiveDirectRoutingQueue()
        {
            var exchangeName = "DirectRoutingExchangeName_MyApp";
            var queueName = "DirectRoutingQueue2_MyApp";
            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchangeName, "direct");
                    var queuName = channel.QueueDeclare(queueName, false, false, false, null);
                    channel.QueueBind(queuName, exchangeName, "order");
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var order = body.Deserialize<Order>();
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine(" [x] Received {0}, {1}", order.Id, order.TotalValue);
                    };

                    channel.BasicConsume(queue: queuName, autoAck: true, consumer: consumer);
                    Console.WriteLine("Program Finished...");
                    Console.ReadLine();
                }
            }
        }
    }
}
