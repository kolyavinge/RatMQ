using System.Linq;
using RatMQ.Contracts;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(CommitMessageRequestData))]
    public class CommitMessageRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            var commitMessageRequestData = (CommitMessageRequestData)requestData;

            var consumer = brokerContext.Consumers.First(x => x.ClientId == commitMessageRequestData.ClientId);
            var message = brokerContext.Messages.First(x => x.Id == commitMessageRequestData.MessageId);

            consumer.IsReadyToConsume = true;
            message.IsCommited = true;

            return new CommitMessageResponseData { Success = true };
        }
    }
}
