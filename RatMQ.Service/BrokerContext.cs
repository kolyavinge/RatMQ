using System.Collections.Concurrent;
using System.Collections.Generic;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public class BrokerContext
    {
        public ConcurrentBag<Queue> Queues { get; private set; }

        public ConcurrentBag<Client> Clients { get; private set; }

        public ConcurrentBag<Consumer> Consumers { get; set; }

        public ConcurrentBag<BrokerMessage> Messages { get; private set; }

        public BrokerContext()
        {
            Queues = new ConcurrentBag<Queue>();
            Clients = new ConcurrentBag<Client>();
            Consumers = new ConcurrentBag<Consumer>();
            Messages = new ConcurrentBag<BrokerMessage>();
        }
    }
}
