using System.Linq;
using RatMQ.Contracts;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(QueueExistRequestData))]
    public class QueueExistRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            var queueExistRequestData = (QueueExistRequestData)requestData;
            return new QueueExistResponseData { Success = brokerContext.Queues.Any(x => x.Name == queueExistRequestData.QueueName) };
        }
    }
}
