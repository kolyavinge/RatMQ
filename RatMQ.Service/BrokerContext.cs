using System.Collections.Generic;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public class BrokerContext
    {
        public List<Client> Clients { get; private set; }

        public List<Consumer> Consumers { get; private set; }

        public List<BrokerMessage> Messages { get; private set; }

        public BrokerContext()
        {
            Clients = new List<Client>();
            Consumers = new List<Consumer>();
            Messages = new List<BrokerMessage>();
        }
    }
}
