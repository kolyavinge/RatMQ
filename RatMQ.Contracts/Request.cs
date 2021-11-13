using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    [Serializable]
    public class Request
    {
        public string DataType { get; set; }
        public byte[] Data { get; set; }
    }
}
