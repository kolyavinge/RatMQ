using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class QueueExistRequestData
    {
        public string QueueName { get; set; }
    }

    [Serializable]
    public class QueueExistResponseData
    {
        public bool Success { get; set; }
    }
}
