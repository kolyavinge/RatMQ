using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RatMQ.Service.DataAccess;

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
                    services.AddSingleton<IDataContextLoader, DataContextLoader>();
                    services.AddSingleton<IDataContext>(serviceProvider => serviceProvider.GetService<IDataContextLoader>().GetDataContext());
                    services.AddSingleton<IBrokerMessageStorage, BrokerMessageStorage>();
                    services.AddSingleton<IBrokerContext>(serviceProvider => new BrokerContextLoader(serviceProvider.GetService<IBrokerMessageStorage>()).GetBrokerContext());
                    services.AddSingleton<IConsumerMessageSender, ConsumerMessageSender>();
                    services.AddSingleton<IRequestDataProcessorFactory>(serviceProvider => new RequestDataProcessorFactory(serviceProvider));
                    services.AddHostedService<ServiceWorker>();
                });
    }
}
