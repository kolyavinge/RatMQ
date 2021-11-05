using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(GetQueueRequestData))]
    public class GetQueueRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            return new GetQueueResponseData { Success = true };
        }
    }
}
