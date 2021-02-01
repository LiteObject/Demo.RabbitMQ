namespace Demo.Subscriber.Console
{
    using Microsoft.Extensions.Hosting;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A timed background task makes use of the System.Threading.Timer class. 
    /// The timer triggers the task's DoWork method. The timer is disabled on 
    /// StopAsync and disposed when the service container is disposed on Dispose
    ///  
    /// SOURCE: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio
    /// </summary>
    public class QueueMonitorService : IHostedService, IDisposable
    {
        private bool disposedValue;
        private Timer _timer;

        public QueueMonitorService() 
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(QueueMonitorService)} is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(QueueMonitorService)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Console.WriteLine($"{ nameof(QueueMonitorService)} is working.");
            Console.WriteLine("Consuming Queue Now");

            // SOURCE: https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
            ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";

            using IConnection conn = factory.CreateConnection();
            using IModel channel = conn.CreateModel();
            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine(" [x] Received from Rabbit: {0}", message);
            };

            channel.BasicConsume(queue: "hello",
                                    autoAck: true,
                                    consumer: consumer);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // TODO: dispose managed state (managed objects)
                if (disposing)
                {                    
                    /*  Call Dispose() on other objects owned by this instance.
                        You can reference other finalizable objects here. */

                    _timer?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~QueueMonitorService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            // Prevent finalizer from running.
            GC.SuppressFinalize(this);
        }      
    }
}
