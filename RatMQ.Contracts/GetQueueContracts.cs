using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class GetQueueRequestData
    {
        public string QueueName { get; set; }
    }

    public class GetQueueResponseData
    {
        public bool Success { get; set; }
    }
}
