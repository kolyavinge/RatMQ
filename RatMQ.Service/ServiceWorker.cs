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
            var activeMessages = from message in _brokerContext.Messages
                                 join consumer in _brokerContext.Consumers on message.QueueName equals consumer.QueueName
                                 where !message.IsSended && !message.IsCommited && consumer.IsReadyToConsume
                                 select new { message, consumer };
            foreach (var activeMessage in activeMessages)
            {
                var client = _brokerContext.Clients.First(x => x.ClientId == activeMessage.consumer.ClientId);
                SendToConsumer(client, activeMessage.message);
                activeMessage.message.IsSended = true;
                activeMessage.consumer.IsReadyToConsume = false;
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

        private void SendToConsumer(Client client, BrokerMessage message)
        {
            var clientMessageJson = JsonSerializer.ToJson(new ClientMessage
            {
                Id = message.Id,
                Body = message.Body
            });
            using var consumerTcpClient = new TcpClient();
            {
                consumerTcpClient.Connect(client.ClientIp, client.ClientPort);
                using (var stream = consumerTcpClient.GetStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(clientMessageJson);
                    stream.Write(bytes);
                }
            }
        }
    }
}
