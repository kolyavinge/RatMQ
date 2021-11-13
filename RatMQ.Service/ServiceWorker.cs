using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RatMQ.Contracts;
using RatMQ.Service.Domain;
using RatMQ.Service.Utils;

namespace RatMQ.Service
{
    public class ServiceWorker : BackgroundService
    {
        private readonly IBrokerContext _brokerContext;
        private readonly IConsumerMessageSender _consumerMessageSender;
        private readonly IRequestDataProcessorFactory _requestDataProcessorFactory;
        private readonly ILogger<ServiceWorker> _logger;
        private readonly int _port;

        public ServiceWorker(
            IBrokerContext brokerContext,
            IConsumerMessageSender consumerMessageSender,
            IRequestDataProcessorFactory requestDataProcessorFactory,
            IConfiguration configuration,
            ILogger<ServiceWorker> logger)
        {
            _brokerContext = brokerContext;
            _consumerMessageSender = consumerMessageSender;
            _requestDataProcessorFactory = requestDataProcessorFactory;
            _logger = logger;
            _port = configuration.GetValue<int>("Port");
            ReadQueueDescriptions();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumerMessageSender.StartAsync();
            var ipAddress = IPAddress.Parse("127.0.0.1");
            var tcpListener = new TcpListener(ipAddress, _port);
            tcpListener.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                var tcpClient = tcpListener.AcceptTcpClient();
                Task.Factory.StartNew(() => ProcessRequest(tcpClient), stoppingToken);
            }
        }

        private void ProcessRequest(TcpClient tcpClient)
        {
            NetworkStream stream = null;
            try
            {
                var resultBuffer = new byte[10 * 1024 * 1024];
                var readBuffer = new byte[1024 * 1024];
                stream = tcpClient.GetStream();
                var binaryReader = new BinaryReader(stream);
                int requestSize = binaryReader.ReadInt32();

                int resultBufferCurrentLength = 0;
                int readBufferLength;
                for (int i = 0; resultBufferCurrentLength != requestSize; i++)
                {
                    readBufferLength = stream.Read(readBuffer);
                    if (readBufferLength + resultBufferCurrentLength > resultBuffer.Length)
                    {
                        Array.Resize(ref resultBuffer, 2 * resultBuffer.Length);
                    }
                    Array.Copy(readBuffer, 0, resultBuffer, resultBufferCurrentLength, readBufferLength);
                    resultBufferCurrentLength += readBufferLength;
                }

                var requestData = BinarySerializer.FromBinary(resultBuffer, resultBufferCurrentLength);
                var requestDataProcessor = _requestDataProcessorFactory.GetProcessorFor(requestData);
                var responseData = requestDataProcessor.GetResponseData(_brokerContext, requestData);
                var responseDataBytes = BinarySerializer.ToBinary(responseData);
                var binaryWriter = new BinaryWriter(stream);
                binaryWriter.Write(responseDataBytes.Length);
                stream.Write(responseDataBytes);
                stream.Flush();

                _consumerMessageSender.SendMessagesToConsumersIfNeeded();
            }
            finally
            {
                stream.Close();
                tcpClient.Close();
            }
        }

        private void ReadQueueDescriptions()
        {
            var getQueueDescriptions = QueueFileReader.GetQueueDescriptions();
            if (getQueueDescriptions != null)
            {
                _brokerContext.Queues.AddRange(getQueueDescriptions.Select(x => new Queue { Name = x.Name }));
            }
        }
    }
}
