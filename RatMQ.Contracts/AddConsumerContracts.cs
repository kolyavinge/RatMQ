using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class AddConsumerRequestData
    {
        public string ClientId { get; set; }
        public string QueueName { get; set; }
    }

    public class AddConsumerResponseData
    {
        public bool Success { get; set; }
    }
}
