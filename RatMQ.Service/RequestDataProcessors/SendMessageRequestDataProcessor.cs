using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(SendMessageRequestData))]
    public class SendMessageRequestDataProcessor : RequestDataProcessor
    {
        private readonly IConsumerMessageSender _consumerMessageSender;
        private readonly IBrokerMessageStorage _brokerMessageStorage;

        public SendMessageRequestDataProcessor(IConsumerMessageSender consumerMessageSender, IBrokerMessageStorage brokerMessageStorage)
        {
            _consumerMessageSender = consumerMessageSender;
            _brokerMessageStorage = brokerMessageStorage;
        }

        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var sendMessageRequestData = (SendMessageRequestData)requestData;

            var brokerMessage = new BrokerMessage
            {
                Id = Guid.NewGuid().ToString(),
                QueueName = sendMessageRequestData.QueueName,
                Headers = sendMessageRequestData.Headers,
                Body = sendMessageRequestData.MessageBody,
            };
            brokerContext.Messages.Add(brokerMessage);

            lock (brokerContext)
            {
                _brokerMessageStorage.SaveUncommitedMessage(brokerMessage);
            }

            _consumerMessageSender.CheckToSend();

            return new SendMessageResponseData { Success = true };
        }
    }
}
