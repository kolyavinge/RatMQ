using System.Collections.Generic;
using System.Linq;
using System.Text;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public class Broker
    {
        private readonly ConnectionContext _connectionContext;
        private readonly Dictionary<string, Queue> _queues;

        internal Broker(ConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
            _queues = new Dictionary<string, Queue>();
            _connectionContext.ListenToBroker(ListenToBrokerCallback);
        }

        private void ListenToBrokerCallback(byte[] buffer, int count)
        {
            var clientMessage = (ClientMessage)BinarySerializer.FromBinary(buffer, count);
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

        public Queue CreateQueue(string queueName)
        {
            var request = new CreateQueueRequestData
            {
                QueueName = queueName
            };
            var response = _connectionContext.SendToBroker<CreateQueueResponseData>(request);
            if (response.Success)
            {
                var queue = new Queue(_connectionContext, queueName);
                _queues.Add(queueName, queue);
                return queue;
            }
            else
            {
                throw new RatMQException();
            }
        }

        public Queue GetQueue(string queueName)
        {
            var request = new GetQueueRequestData
            {
                QueueName = queueName
            };
            var response = _connectionContext.SendToBroker<GetQueueResponseData>(request);
            if (response.Success)
            {
                Queue queue;
                if (_queues.ContainsKey(queueName))
                {
                    queue = _queues[queueName];
                }
                else
                {
                    queue = new Queue(_connectionContext, queueName);
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
