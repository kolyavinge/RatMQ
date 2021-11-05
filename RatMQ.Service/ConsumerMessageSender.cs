using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RatMQ.Contracts;
using RatMQ.Service.Domain;
using RatMQ.Service.Utils;

namespace RatMQ.Service
{
    class ConsumerMessageSender
    {
        private readonly TimeSpan _sendMessageTimeout;
        private readonly BrokerContext _brokerContext;

        public ConsumerMessageSender(BrokerContext brokerContext)
        {
            _brokerContext = brokerContext;
            _sendMessageTimeout = TimeSpan.FromSeconds(30);
        }

        public void StartAsync()
        {
            Task.Run(Start);
        }

        private void Start()
        {
            while (true)
            {
                SendMessagesToConsumers();
                Thread.Sleep(_sendMessageTimeout);
                _brokerContext.Consumers.Where(x => !x.IsReadyToConsume).Each(x => x.IsReadyToConsume = true);
                _brokerContext.Messages.Where(x => x.IsSended && !x.IsCommited).Each(x => x.IsSended = false);
            }
        }

        public void SendMessagesToConsumers()
        {
            foreach (var message in _brokerContext.Messages.Where(x => !x.IsSended))
            {
                var consumer = _brokerContext.Consumers.Where(c => c.QueueName == message.QueueName && c.IsReadyToConsume).FirstOrDefault();
                if (consumer != null)
                {
                    var client = _brokerContext.Clients.First(c => c.ClientId == consumer.ClientId);
                    SendToConsumer(client, message);
                    message.IsSended = true;
                    consumer.IsReadyToConsume = false;
                    _brokerContext.Consumers.Remove(consumer);
                    _brokerContext.Consumers.Add(consumer);
                }
            }
        }

        private void SendToConsumer(Client client, BrokerMessage message)
        {
            var clientMessageJson = JsonSerializer.ToJson(new ClientMessage
            {
                Id = message.Id,
                QueueName = message.QueueName,
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
