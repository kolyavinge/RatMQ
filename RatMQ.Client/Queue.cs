using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public abstract class AbstractQueue
    {
        internal readonly ConnectionContext _connectionContext;

        internal string QueueName { get; }

        internal AbstractQueue(ConnectionContext connectionContext, string queueName)
        {
            _connectionContext = connectionContext;
            QueueName = queueName;
        }

        internal abstract void SendToConsumers(ClientMessage clientMessage);
    }

    public class Queue<TMessage> : AbstractQueue
    {
        private readonly List<IConsumer<TMessage>> _consumers;

        internal Queue(ConnectionContext connectionContext, string queueName) : base(connectionContext, queueName)
        {
            _consumers = new List<IConsumer<TMessage>>();
        }

        public void SendMessage(TMessage message)
        {
            var request = new SendMessageRequestData
            {
                QueueName = QueueName,
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

        internal override void SendToConsumers(ClientMessage clientMessage)
        {
            var message = (TMessage)JsonSerializer.FromJson(typeof(TMessage), clientMessage.Body);
            foreach (var consumer in _consumers)
            {
                var result = new ConsumeMessageResult(_connectionContext, clientMessage.Id);
                consumer.ConsumeMessage(message, result);
            }
        }
    }
}
