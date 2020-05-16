using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedDomain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumerApp
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


        static void ReceiveStandardQueue()
        {
            var queueName = "StandardQueue_MyApp";

            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var results = channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


                    var consumer = new EventingBasicConsumer(channel);

                    //for (var i = 0; i < results.MessageCount; i++)
                    //{
                    //    BasicGetResult result = channel.BasicGet(queueName, true);
                    //    if (result != null)
                    //    {
                    //        IBasicProperties props = result.BasicProperties;
                    //        var body = result.Body.ToArray();
                    //        var payment = body.Deserialize<Payment>();
                    //        Console.WriteLine(" [x] Received {0}, {1}, {2}", payment.Id, payment.Amount, payment.PaymentType);

                    //    }
                    //}


                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var payment = body.Deserialize<Payment>();
                        Console.WriteLine(" [x] Received {0}, {1}, {2}", payment.Id, payment.Amount, payment.PaymentType);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine("Program Finished...");
                    Console.ReadLine();
                }
            }
        }

        static void ReceiveWorkerQueue()
        {
            var queueName = "WorkerQueue_MyApp";
            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var results = channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var payment = body.Deserialize<Payment>();
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine(" [x] Received {0}, {1}, {2}", payment.Id, payment.Amount, payment.PaymentType);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.WriteLine("Program Finished...");
                    Console.ReadLine();
                }
            }
        }

        static void ReceiveFanoutQueue()
        {
            var exchangeName = "FanoutQueueExchangeName_MyApp";
            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchangeName, "fanout");
                    var queuName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queuName, exchangeName, "");

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var payment = body.Deserialize<Payment>();
                        Console.WriteLine(" [x] Received {0}, {1}, {2}", payment.Id, payment.Amount, payment.PaymentType);
                    };

                    channel.BasicConsume(queue: queuName, autoAck: true, consumer: consumer);
                    Console.WriteLine("Program Finished...");
                    Console.ReadLine();
                }
            }
        }

        static void ReceiveDirectRoutingQueue()
        {
            var exchangeName = "DirectRoutingExchangeName_MyApp";
            var queueName = "DirectRoutingQueue1_MyApp";
            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    channel.ExchangeDeclare(exchangeName, "direct");
                    var queuName = channel.QueueDeclare(queueName, false, false, false, null);
                    channel.QueueBind(queuName, exchangeName, "payment");
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var payment = body.Deserialize<Payment>();
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine(" [x] Received {0}, {1}, {2}", payment.Id, payment.Amount, payment.PaymentType);
                    };

                    channel.BasicConsume(queue: queuName, autoAck: true, consumer: consumer);
                    Console.WriteLine("Program Finished...");
                    Console.ReadLine();
                }
            }
        }

    }

}
