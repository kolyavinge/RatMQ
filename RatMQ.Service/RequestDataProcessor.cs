using System;
using RatMQ.Service.Domain;

namespace RatMQ.Service
{
    public abstract class RequestDataProcessor
    {
        public abstract object GetResponseData(BrokerContext brokerContext, object requestData);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RequestDataProcessorAttribute : Attribute
    {
        public RequestDataProcessorAttribute(Type requestDataType)
        {
            RequestDataType = requestDataType;
        }

        public Type RequestDataType { get; }
    }
}
