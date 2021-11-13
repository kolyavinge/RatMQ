using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(AddConsumerRequestData))]
    public class AddConsumerRequestDataProcessor : RequestDataProcessor
    {
        private readonly IConsumerMessageSender _consumerMessageSender;

        public AddConsumerRequestDataProcessor(IConsumerMessageSender consumerMessageSender)
        {
            _consumerMessageSender = consumerMessageSender;
        }

        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var addConsumerRequestData = (AddConsumerRequestData)requestData;
            brokerContext.Consumers.Add(new Consumer
            {
                ClientId = addConsumerRequestData.ClientId,
                QueueName = addConsumerRequestData.QueueName
            });

            _consumerMessageSender.CheckToSend();

            return new AddConsumerResponseData { Success = true };
        }
    }
}
