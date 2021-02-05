namespace Demo.Subscriber.Console
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;
    using System;

    /// <summary>
    /// SOURCE:
    /// https://www.rabbitmq.com/dotnet-api-guide.html
    /// https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html
    /// </summary>
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<QueueMonitorService>();
            });
    }
}
