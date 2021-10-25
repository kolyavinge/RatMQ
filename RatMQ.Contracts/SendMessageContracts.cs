using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class SendMessageRequestData
    {
        public string QueueName { get; set; }
        public string Message { get; set; }
    }

    public class SendMessageResponseData
    {
        public bool Success { get; set; }
    }
}
