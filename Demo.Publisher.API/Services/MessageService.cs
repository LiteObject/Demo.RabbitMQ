using RabbitMQ.Client;
using System;
using System.Text;

namespace Demo.Publisher.API.Services
{
    /// <summary>
    /// SOURCE: https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
    /// </summary>
    public class MessageService : IMessageService
    {
        ConnectionFactory _factory;
        IConnection _conn;
        IModel _channel;

        public MessageService()
        {
            _factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";

            try
            {
                _conn = _factory.CreateConnection();
                _channel = _conn.CreateModel();

                _channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool Enqueue(string message)
        {
            var body = Encoding.UTF8.GetBytes("server processed " + message);
            _channel.BasicPublish(exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);
            Console.WriteLine(" [x] Published {0} to RabbitMQ", message);
            return true;
        }
    }
}
