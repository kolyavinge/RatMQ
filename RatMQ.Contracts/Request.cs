using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class Request
    {
        public string JsonDataTypeName { get; set; }
        public string JsonData { get; set; }
    }
}
