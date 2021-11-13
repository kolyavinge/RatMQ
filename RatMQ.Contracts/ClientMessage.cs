using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class ClientMessage
    {
        public string Id { get; set; }

        public string QueueName { get; set; }

        public KeyValuePair<string, object>[] Headers { get; set; }

        public byte[] Body { get; set; }
    }
}
