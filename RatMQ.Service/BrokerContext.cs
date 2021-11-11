using System.Collections.Concurrent;
using System.Collections.Generic;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public interface IBrokerContext
    {
        ConcurrentBag<Queue> Queues { get; set; }

        ConcurrentBag<Client> Clients { get; set; }

        ConcurrentBag<Consumer> Consumers { get; set; }

        ConcurrentBag<BrokerMessage> Messages { get; set; }
    }

    public class BrokerContext : IBrokerContext
    {
        public ConcurrentBag<Queue> Queues { get; set; }

        public ConcurrentBag<Client> Clients { get; set; }

        public ConcurrentBag<Consumer> Consumers { get; set; }

        public ConcurrentBag<BrokerMessage> Messages { get; set; }

        public BrokerContext()
        {
            Queues = new ConcurrentBag<Queue>();
            Clients = new ConcurrentBag<Client>();
            Consumers = new ConcurrentBag<Consumer>();
            Messages = new ConcurrentBag<BrokerMessage>();
        }
    }
}
