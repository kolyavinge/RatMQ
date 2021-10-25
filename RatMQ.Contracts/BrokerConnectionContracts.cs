using System;

namespace RatMQ.Contracts
{
    public class BrokerConnectionRequestData
    {
        public string BrokerIp { get; set; }
        public int BrokerPort { get; set; }
        public string ClientId { get; set; }
        public string ClientIp { get; set; }
        public int ClientPort { get; set; }
    }

    public class BrokerConnectionResponseData
    {
        public bool Success { get; set; }
    }
}
