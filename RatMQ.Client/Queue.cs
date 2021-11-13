using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public class Queue
    {
        private readonly ConnectionContext _connectionContext;
        private readonly List<IConsumer> _consumers;

        internal string QueueName { get; }

        internal Queue(ConnectionContext connectionContext, string queueName)
        {
            _connectionContext = connectionContext;
            QueueName = queueName;
            _consumers = new List<IConsumer>();
        }

        public void SendMessage(KeyValuePair<string, object>[] headers, byte[] message)
        {
            var request = new SendMessageRequestData
            {
                QueueName = QueueName,
                Headers = headers,
                MessageBody = message
            };
            var response = _connectionContext.SendToBroker<SendMessageResponseData>(request);
            if (!response.Success)
            {
            }
        }

        public void AddConsumer(IConsumer consumer)
        {
            var request = new AddConsumerRequestData
            {
                ClientId = _connectionContext.ClientId,
                QueueName = QueueName
            };
            var response = _connectionContext.SendToBroker<AddConsumerResponseData>(request);
            if (response.Success)
            {
                _consumers.Add(consumer);
            }
            else
            {

            }
        }

        internal void SendToConsumers(ClientMessage clientMessage)
        {
            foreach (var consumer in _consumers)
            {
                var result = new ConsumeMessageResult(_connectionContext, clientMessage.Id);
                consumer.ConsumeMessage(clientMessage, result);
            }
        }
    }
}
