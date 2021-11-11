using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(BrokerConnectionRequestData))]
    public class BrokerConnectionRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var brokerConnectionRequestData = (BrokerConnectionRequestData)requestData;
            brokerContext.Clients.Add(new Client
            {
                ClientId = brokerConnectionRequestData.ClientId,
                ClientIp = brokerConnectionRequestData.ClientIp,
                ClientPort = brokerConnectionRequestData.ClientPort
            });

            return new BrokerConnectionResponseData { Success = true };
        }
    }
}
