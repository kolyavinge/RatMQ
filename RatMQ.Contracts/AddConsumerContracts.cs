using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class AddConsumerRequestData
    {
        public string ClientId { get; set; }
        public string QueueName { get; set; }
    }

    [Serializable]
    public class AddConsumerResponseData
    {
        public bool Success { get; set; }
    }
}
