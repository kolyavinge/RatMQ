using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class SendMessageRequestData
    {
        public string QueueName { get; set; }
        public KeyValuePair<string, object>[] Headers { get; set; }
        public byte[] MessageBody { get; set; }
    }

    [Serializable]
    public class SendMessageResponseData
    {
        public bool Success { get; set; }
    }
}
