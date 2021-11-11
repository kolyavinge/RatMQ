using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(SendMessageRequestData))]
    public class SendMessageRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var sendMessageRequestData = (SendMessageRequestData)requestData;
            brokerContext.Messages.Add(new BrokerMessage
            {
                Id = Guid.NewGuid().ToString(),
                Body = sendMessageRequestData.Message,
                QueueName = sendMessageRequestData.QueueName
            });

            return new SendMessageResponseData { Success = true };
        }
    }
}
