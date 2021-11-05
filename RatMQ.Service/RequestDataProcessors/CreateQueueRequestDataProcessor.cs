using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(CreateQueueRequestData))]
    public class CreateQueueRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            var createQueueRequestData = (CreateQueueRequestData)requestData;
            brokerContext.Queues.Add(new Queue { Name = createQueueRequestData.QueueName });
            return new CreateQueueResponseData { Success = true };
        }
    }
}
