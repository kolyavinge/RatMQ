﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public class Broker
    {
        private readonly ConnectionContext _connectionContext;
        private readonly Dictionary<string, AbstractQueue> _queues;

        internal Broker(ConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
            _queues = new Dictionary<string, AbstractQueue>();
            _connectionContext.ListenToBroker(ListenToBrokerCallback);
        }

        private void ListenToBrokerCallback(byte[] buffer, int count)
        {
            var json = Encoding.UTF8.GetString(buffer, 0, count);
            var clientMessage = JsonSerializer.FromJson<ClientMessage>(json);
            var queue = _queues[clientMessage.QueueName];
            queue.SendToConsumers(clientMessage);
        }

        public bool IsQueueExist(string queueName)
        {
            var request = new QueueExistRequestData
            {
                QueueName = queueName
            };
            var response = _connectionContext.SendToBroker<QueueExistResponseData>(request);
            if (response.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Queue<TMessage> CreateQueue<TMessage>(string queueName)
        {
            var request = new CreateQueueRequestData
            {
                QueueName = queueName
            };
            var response = _connectionContext.SendToBroker<CreateQueueResponseData>(request);
            if (response.Success)
            {
                var queue = new Queue<TMessage>(_connectionContext, queueName);
                _queues.Add(queueName, queue);
                return queue;
            }
            else
            {
                throw new RatMQException();
            }
        }

        public Queue<TMessage> GetQueue<TMessage>(string queueName)
        {
            var request = new GetQueueRequestData
            {
                QueueName = queueName
            };
            var response = _connectionContext.SendToBroker<GetQueueResponseData>(request);
            if (response.Success)
            {
                Queue<TMessage> queue;
                if (_queues.ContainsKey(queueName))
                {
                    queue = (Queue<TMessage>)_queues[queueName];
                }
                else
                {
                    queue = new Queue<TMessage>(_connectionContext, queueName);
                    _queues.Add(queueName, queue);
                }

                return queue;
            }
            else
            {
                throw new RatMQException();
            }
        }
    }
}
