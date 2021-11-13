using System.Collections.Generic;

namespace RatMQ.Service.Domain
{
    public class BrokerMessage
    {
        public string Id { get; set; }

        public KeyValuePair<string, object>[] Headers { get; set; }

        public byte[] Body { get; set; }

        public bool IsSended { get; set; }

        public bool IsCommited { get; set; }

        public string QueueName { get; set; }

        public BrokerMessage()
        {
            IsSended = false;
            IsCommited = false;
        }
    }
}
