using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(SendMessageRequestData))]
    public class SendMessageRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            var sendMessageRequestData = (SendMessageRequestData)requestData;
            brokerContext.Messages.Add(new BrokerMessage { Body = sendMessageRequestData.Message });

            return new SendMessageResponseData { Success = true };
        }
    }
}
