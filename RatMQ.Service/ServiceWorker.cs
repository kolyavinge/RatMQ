using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RatMQ.Contracts;
using RatMQ.Service.Utils;

namespace RatMQ.Service
{
    public class ServiceWorker : BackgroundService
    {
        private readonly BrokerContext _brokerContext;
        private readonly RequestDataProcessorFactory _requestDataProcessorFactory;
        private readonly ConsumerMessageSender _consumerMessageSender;
        private readonly ILogger<ServiceWorker> _logger;
        private readonly int _port;

        public ServiceWorker(IConfiguration configuration, ILogger<ServiceWorker> logger)
        {
            _brokerContext = new BrokerContext();
            _requestDataProcessorFactory = new RequestDataProcessorFactory();
            _consumerMessageSender = new ConsumerMessageSender(_brokerContext);
            _logger = logger;
            _port = configuration.GetValue<int>("Port");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumerMessageSender.StartAsync();
            var buffer = new byte[1024];
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var server = new TcpListener(ipAddress, _port);
            server.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                var client = server.AcceptTcpClient();
                using (var stream = client.GetStream())
                {
                    var count = stream.Read(buffer);
                    var requestData = RequestDataFromBytes(buffer, count);
                    var requestDataProcessor = _requestDataProcessorFactory.GetProcessorFor(requestData);
                    var responseData = requestDataProcessor.GetResponseData(_brokerContext, requestData);
                    stream.Write(ResponseDataToBytes(responseData));
                }
                _consumerMessageSender.SendMessagesToConsumers();
            }
        }

        private object RequestDataFromBytes(byte[] buffer, int count)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, count);
            var request = JsonSerializer.FromJson<Request>(json);
            var requestDataType = Type.GetType(request.JsonDataTypeName);

            return JsonSerializer.FromJson(requestDataType, request.JsonData);
        }

        private byte[] ResponseDataToBytes(object responseData)
        {
            var response = new Response();
            response.JsonData = JsonSerializer.ToJson(responseData);
            var json = JsonSerializer.ToJson(response);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
