using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RatMQ.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var brokerContext = new BrokerContext();
                    services.AddSingleton<IBrokerContext>(brokerContext);
                    services.AddSingleton<IConsumerMessageSender>(new ConsumerMessageSender(brokerContext));
                    services.AddSingleton<IRequestDataProcessorFactory>(new RequestDataProcessorFactory(services));
                    services.AddHostedService<ServiceWorker>();
                });
    }
}
