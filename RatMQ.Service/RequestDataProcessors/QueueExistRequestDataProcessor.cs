using System;
using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(QueueExistRequestData))]
    public class QueueExistRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            return new QueueExistResponseData { Success = true };
        }
    }
}
