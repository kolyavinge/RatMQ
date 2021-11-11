using System.Linq;
using RatMQ.Contracts;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(GetQueueRequestData))]
    public class GetQueueRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(IBrokerContext brokerContext, object requestData)
        {
            var getQueueRequestData = (GetQueueRequestData)requestData;
            return new GetQueueResponseData { Success = brokerContext.Queues.Any(x => x.Name == getQueueRequestData.QueueName) };
        }
    }
}
