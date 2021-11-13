using System;
using System.Collections.Concurrent;
using System.IO;
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
    public interface IConsumerMessageSender
    {
        void StartAsync();
        void CheckToSend();
        void SendMessagesToConsumersIfNeeded();
        void SendMessagesToConsumers();
    }

    public class ConsumerMessageSender : IConsumerMessageSender
    {
        private readonly TimeSpan _sendMessageTimeout;
        private readonly IBrokerContext _brokerContext;
        private bool _needToSend;

        public ConsumerMessageSender(IBrokerContext brokerContext)
        {
            _brokerContext = brokerContext;
            _sendMessageTimeout = TimeSpan.FromSeconds(30);
            _needToSend = false;
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
                lock (_brokerContext)
                {
                    _brokerContext.Consumers.Where(x => !x.IsReadyToConsume).Each(x => x.IsReadyToConsume = true);
                    _brokerContext.Messages.Where(x => x.IsSended && !x.IsCommited).Each(x => x.IsSended = false);
                }
            }
        }

        public void CheckToSend()
        {
            _needToSend = true;
        }

        public void SendMessagesToConsumersIfNeeded()
        {
            if (_needToSend)
            {
                SendMessagesToConsumers();
                _needToSend = false;
            }
        }

        public void SendMessagesToConsumers()
        {
            lock (_brokerContext)
            {
                foreach (var message in _brokerContext.Messages.Where(x => !x.IsSended).ToList())
                {
                    var consumer = _brokerContext.Consumers.Where(c => c.QueueName == message.QueueName && c.IsReadyToConsume).FirstOrDefault();
                    if (consumer != null)
                    {
                        var client = _brokerContext.Clients.First(c => c.ClientId == consumer.ClientId);
                        SendToConsumer(client, message);
                        message.IsSended = true;
                        consumer.IsReadyToConsume = false;
                        if (_brokerContext.Consumers.Count > 1)
                        {
                            _brokerContext.Consumers = new ConcurrentBag<Consumer>(_brokerContext.Consumers.Except(new[] { consumer }));
                            _brokerContext.Consumers.Add(consumer);
                        }
                    }
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
                    var binaryWriter = new BinaryWriter(stream);
                    binaryWriter.Write(bytes.Length);
                    stream.Write(bytes);
                }
            }
        }
    }
}
