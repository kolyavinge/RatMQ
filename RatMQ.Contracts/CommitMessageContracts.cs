using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class CommitMessageRequestData
    {
        public string ClientId { get; set; }
        public string MessageId { get; set; }
    }

    [Serializable]
    public class CommitMessageResponseData
    {
        public bool Success { get; set; }
    }
}
