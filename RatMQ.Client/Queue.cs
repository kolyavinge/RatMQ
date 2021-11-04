using System;
using System.Collections.Generic;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public class Queue<TMessage>
    {
        private readonly ConnectionContext _connectionContext;
        private readonly string _queueName;
        private readonly List<IConsumer<TMessage>> _consumers;

        internal Queue(ConnectionContext connectionContext, string queueName)
        {
            _connectionContext = connectionContext;
            _queueName = queueName;
            _consumers = new List<IConsumer<TMessage>>();
        }

        public void SendMessage(TMessage message)
        {
            var request = new SendMessageRequestData
            {
                QueueName = _queueName,
                Message = JsonSerializer.ToJson(message)
            };
            var response = _connectionContext.SendToBroker<SendMessageResponseData>(request);
            if (!response.Success)
            {
            }
        }

        public void AddConsumer(IConsumer<TMessage> consumer)
        {
            var request = new AddConsumerRequestData
            {
                ClientId = _connectionContext.ClientId,
                QueueName = _queueName
            };
            var response = _connectionContext.SendToBroker<AddConsumerResponseData>(request);
            if (response.Success)
            {
                _consumers.Add(consumer);
                _connectionContext.ListenToBroker(ListenToBrokerCallback);
            }
            else
            {

            }
        }

        private void ListenToBrokerCallback(byte[] buffer, int count)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, count);
            var clientMessage = JsonSerializer.FromJson<ClientMessage>(json);
            var message = (TMessage)JsonSerializer.FromJson(typeof(TMessage), clientMessage.Body);
            foreach (var consumer in _consumers)
            {
                var result = new ConsumeMessageResult(_connectionContext, clientMessage.Id);
                consumer.ConsumeMessage(message, result);
            }
        }
    }
}
