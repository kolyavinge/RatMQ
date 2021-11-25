using System.Linq;
using RatMQ.Contracts;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(CommitMessageRequestData))]
    public class CommitMessageRequestDataProcessor : RequestDataProcessor
    {
        private readonly IConsumerMessageSender _consumerMessageSender;
        private readonly IBrokerMessageStorage _brokerMessageStorage;

        public CommitMessageRequestDataProcessor(IConsumerMessageSender consumerMessageSender, IBrokerMessageStorage brokerMessageStorage)
        {
            _consumerMessageSender = consumerMessageSender;
            _brokerMessageStorage = brokerMessageStorage;
        }

        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var commitMessageRequestData = (CommitMessageRequestData)requestData;

            lock (brokerContext)
            {
                var message = brokerContext.Messages.First(x => x.Id == commitMessageRequestData.MessageId);
                var consumer = brokerContext.Consumers.First(x => x.ClientId == commitMessageRequestData.ClientId && x.QueueName == message.QueueName);
                consumer.IsReadyToConsume = true;
                message.IsCommited = true;
                _brokerMessageStorage.CommitMessage(message.Id);
            }

            _consumerMessageSender.CheckToSend();

            return new CommitMessageResponseData { Success = true };
        }
    }
}
