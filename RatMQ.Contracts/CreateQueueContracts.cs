using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class CreateQueueRequestData
    {
        public string QueueName { get; set; }
    }

    public class CreateQueueResponseData
    {
        public bool Success { get; set; }
    }
}
