using RabbitMQ.Client;
using System;
using System.Text;

namespace Demo.Publisher.API.Services
{
    /// <summary>
    /// SOURCE: https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
    /// </summary>
    public class MessageService : IMessageService, IDisposable
    {
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public MessageService()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq", Port = 5672, UserName = "guest", Password = "guest"
            };

            try
            {
                /********************************************************************************
                 * Connections are meant to be long-lived. The underlying protocol is designed
                 * and optimized for long running connections. That means that opening a new
                 * connection per operation, e.g. a message published, is unnecessary and strongly
                 * discouraged as it will introduce a lot of network round-trips and overhead.
                 ********************************************************************************/
                _conn = factory.CreateConnection(); // IConnection represents an AMQP connection
                _channel = _conn.CreateModel(); // AMQP data channel and provides the AMQP operations

                /********************************************************************************************************
                 * If the queue doesn't exists, it will be created
                 * If it already exists, then no effect on the existing queue
                *********************************************************************************************************/
                _channel.ExchangeDeclare("hello", "direct");
                _channel.QueueDeclare(queue: "hello", // Queue name
                                        durable: false, // The queue will survive (recreated) a broker restart, not messages
                                        exclusive: false, // Used by only one connection and the queue will be deleted when that connection closes
                                        autoDelete: false, // Queue that has had at least one consumer is deleted when last consumer unsubscribes
                                        arguments: null); // optional; used by plugins and broker-specific features such as message TTL, queue length limit, etc

                _channel.QueueBind(queue: "hello", exchange: "hello", routingKey: "hello");

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public bool Enqueue(string message)
        {
            var body = Encoding.UTF8.GetBytes($"Server processed {message}");
            _channel.BasicPublish(exchange: "hello",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);
            Console.WriteLine(" [x] Published {0} to RabbitMQ", message);
            return true;
        }

        public void Dispose()
        {
            this._channel.Dispose();
            this._conn.Dispose();
        }
    }
}
