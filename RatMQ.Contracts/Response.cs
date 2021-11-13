using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class Response
    {
        public byte[] Data { get; set; }
    }
}
