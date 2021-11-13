using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(SendMessageRequestData))]
    public class SendMessageRequestDataProcessor : RequestDataProcessor
    {
        private readonly IConsumerMessageSender _consumerMessageSender;

        public SendMessageRequestDataProcessor(IConsumerMessageSender consumerMessageSender)
        {
            _consumerMessageSender = consumerMessageSender;
        }

        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var sendMessageRequestData = (SendMessageRequestData)requestData;

            brokerContext.Messages.Add(new BrokerMessage
            {
                Id = Guid.NewGuid().ToString(),
                Body = sendMessageRequestData.MessageBody,
                Headers = sendMessageRequestData.Headers,
                QueueName = sendMessageRequestData.QueueName
            });

            _consumerMessageSender.CheckToSend();

            return new SendMessageResponseData { Success = true };
        }
    }
}
