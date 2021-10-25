using System.Net;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    public static class BrokerConnector
    {
        public static Broker Connect(string brokerIp, int brokerPort, string clientId, int clientPort)
        {
            var connectionContext = new ConnectionContext(brokerIp, brokerPort, clientId, clientPort);
            var request = new BrokerConnectionRequestData
            {
                BrokerIp = brokerIp,
                BrokerPort = brokerPort,
                ClientId = clientId,
                ClientIp = "127.0.0.1",
                ClientPort = clientPort
            };
            var response = connectionContext.SendToBroker<BrokerConnectionResponseData>(request);
            if (response.Success)
            {
                return new Broker(connectionContext);
            }
            else
            {
                throw new RatMQException();
            }
        }
    }
}
