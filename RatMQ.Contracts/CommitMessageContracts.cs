using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class CommitMessageRequestData
    {
        public string ClientId { get; set; }
        public string MessageId { get; set; }
    }

    public class CommitMessageResponseData
    {
        public bool Success { get; set; }
    }
}
