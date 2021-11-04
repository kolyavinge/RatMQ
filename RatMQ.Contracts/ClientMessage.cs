using System;
using System.Collections.Generic;
using System.Text;

namespace RatMQ.Contracts
{
    public class ClientMessage
    {
        public string Id { get; set; }

        public string Body { get; set; }
    }
}
