using RatMQ.Contracts;
using RatMQ.Service.Domain;

namespace RatMQ.Service.RequestDataProcessors
{
    [RequestDataProcessor(typeof(AddConsumerRequestData))]
    public class AddConsumerRequestDataProcessor : RequestDataProcessor
    {
        public override object GetResponseData(BrokerContext brokerContext, object requestData)
        {
            var addConsumerRequestData = (AddConsumerRequestData)requestData;
            brokerContext.Consumers.Add(new Consumer { ClientId = addConsumerRequestData.ClientId });

            return new AddConsumerResponseData { Success = true };
        }
    }
}
