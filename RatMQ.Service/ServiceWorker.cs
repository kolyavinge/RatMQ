using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RatMQ.Contracts;
using RatMQ.Service.Domain;
using RatMQ.Service.Utils;

namespace RatMQ.Service
{
    public class ServiceWorker : BackgroundService
    {
        private readonly BrokerContext _brokerContext;
        private readonly RequestDataProcessorFactory _requestDataProcessorFactory;
        private readonly ILogger<ServiceWorker> _logger;

        public ServiceWorker(ILogger<ServiceWorker> logger)
        {
            _brokerContext = new BrokerContext();
            _requestDataProcessorFactory = new RequestDataProcessorFactory();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new byte[1024];
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var port = 55555;
            var server = new TcpListener(ipAddress, port);
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
                SendMessagesToConsumers();
            }
        }

        private void SendMessagesToConsumers()
        {
            foreach (var message in _brokerContext.Messages.Where(x => !x.Sended))
            {
                foreach (var consumer in _brokerContext.Consumers)
                {
                    var client = _brokerContext.Clients.First(x => x.ClientId == consumer.ClientId);
                    using (var consumerTcpClient = new TcpClient())
                    {
                        consumerTcpClient.Connect(client.ClientIp, client.ClientPort);
                        using (var stream = consumerTcpClient.GetStream())
                        {
                            var json = JsonSerializer.ToJson(new ClientMessage { Body = message.Body });
                            var bytes = Encoding.UTF8.GetBytes(json);
                            stream.Write(bytes);
                        }
                    }
                }
                message.Sended = true;
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
