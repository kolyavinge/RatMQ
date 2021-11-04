using RatMQ.Contracts;

namespace RatMQ.Client
{
    public class Broker
    {
        private readonly ConnectionContext _connectionContext;

        internal Broker(ConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
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
                return new Queue<TMessage>(_connectionContext, queueName);
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
            var response = _connectionContext.SendToBroker<CommitMessageResponseData>(request);
            if (response.Success)
            {
                return new Queue<TMessage>(_connectionContext, queueName);
            }
            else
            {
                throw new RatMQException();
            }
        }
    }
}
