using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class QueueExistRequestData
    {
        public string QueueName { get; set; }
    }

    public class QueueExistResponseData
    {
        public bool Success { get; set; }
    }
}
