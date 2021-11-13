using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class CreateQueueRequestData
    {
        public string QueueName { get; set; }
    }

    [Serializable]
    public class CreateQueueResponseData
    {
        public bool Success { get; set; }
    }
}
