using System.Collections.Concurrent;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public interface IBrokerContextLoader
    {
        IBrokerContext GetBrokerContext();
    }

    public class BrokerContextLoader : IBrokerContextLoader
    {
        private readonly IBrokerMessageStorage _brokerMessageStorage;

        public BrokerContextLoader(IBrokerMessageStorage brokerMessageStorage)
        {
            _brokerMessageStorage = brokerMessageStorage;
        }

        public IBrokerContext GetBrokerContext()
        {
            var uncommitedMessages = _brokerMessageStorage.GetUncommitedMessages();
            var brokerContext = new BrokerContext
            {
                Messages = new ConcurrentBag<BrokerMessage>(uncommitedMessages)
            };

            return brokerContext;
        }
    }
}
