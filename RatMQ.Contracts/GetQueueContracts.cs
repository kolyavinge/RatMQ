using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class GetQueueRequestData
    {
        public string QueueName { get; set; }
    }

    [Serializable]
    public class GetQueueResponseData
    {
        public bool Success { get; set; }
    }
}
