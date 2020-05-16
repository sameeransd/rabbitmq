using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedDomain;

namespace PublisherApp
{
    class Program
    {
        private static readonly string hostname = "localhost";
        private static readonly string username = "guest";
        private static readonly string password = "guest";

        static void Main(string[] args)
        {

            Console.WriteLine("Program Starting...");
            Console.WriteLine("Enter Queue Message Count...");
            var count = Console.ReadLine();

            while (count != "exit")
            {
                var paymentList = new List<Payment>();
                var orderList = new List<Order>();
                var cc = Convert.ToInt32(count);
                for (int i = 1; i <= cc; i++)
                {
                    paymentList.Add(new Payment(i, i * 100, (i % 2 == 0 ? "CASH" : "CARD")));
                }
                for (int i = 1; i <= cc; i++)
                {
                    orderList.Add(new Order(i, i * 25));
                }
                //SetupStandardQueue(paymentList);
                //SetupWorkerQueue(paymentList);
                //SetupFanoutQueue(paymentList);
                SetupDirectRoutingQueue(paymentList, orderList);

                Console.WriteLine("Success!");
                Console.WriteLine(", Enter Queue Message Count or type exit");
                count = Console.ReadLine();
            }

            Console.WriteLine("Program Finished...");
            Console.ReadLine();
        }

        static void SetupStandardQueue(List<Payment> paymentList)
        {
            var queueName = "StandardQueue_MyApp";

            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


                    foreach (var msg in paymentList)
                    {
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: msg.Serialize());
                    }
                }
            }
        }

        static void SetupWorkerQueue(List<Payment> paymentList)
        {
            var queueName = "WorkerQueue_MyApp";

            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


                    foreach (var msg in paymentList)
                    {
                        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: msg.Serialize());
                    }
                }
            }
        }

        static void SetupFanoutQueue(List<Payment> paymentList)
        {
            var exchangeName = "FanoutQueueExchangeName_MyApp";

            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName,type: "fanout", durable: false, autoDelete: false, arguments: null);


                    foreach (var msg in paymentList)
                    {
                        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: msg.Serialize());
                    }
                }
            }
        }

        static void SetupDirectRoutingQueue(List<Payment> paymentList, List<Order> orderList)
        {
            var exchangeName = "DirectRoutingExchangeName_MyApp";
            var queueName1 = "DirectRoutingQueue1_MyApp";
            var queueName2 = "DirectRoutingQueue2_MyApp";

            var factory = new ConnectionFactory() { HostName = hostname, UserName = username, Password = password };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: false, autoDelete: false, arguments: null);
                    channel.QueueDeclare(queueName1, false, false, false, null);
                    channel.QueueDeclare(queueName2, false, false, false, null);
                    channel.QueueBind(queueName1, exchangeName, "payment");
                    channel.QueueBind(queueName2, exchangeName, "order");

                    foreach (var msg in paymentList)
                    {
                        channel.BasicPublish(exchange: exchangeName, routingKey: "payment", basicProperties: null, body: msg.Serialize());
                    }

                    foreach (var msg in orderList)
                    {
                        channel.BasicPublish(exchange: exchangeName, routingKey: "order", basicProperties: null, body: msg.Serialize());
                    }
                    
                }
            }
        }


    }
}
