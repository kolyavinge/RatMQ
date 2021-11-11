using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RatMQ.Service
{
    public interface IRequestDataProcessorFactory
    {
        RequestDataProcessor GetProcessorFor(object requestData);
    }

    public class RequestDataProcessorFactory : IRequestDataProcessorFactory
    {
        private Dictionary<Type, RequestDataProcessor> _processors;

        public RequestDataProcessorFactory()
        {
            _processors = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.GetCustomAttributes<RequestDataProcessorAttribute>().Any())
                .ToDictionary(
                    k => k.GetCustomAttributes<RequestDataProcessorAttribute>().First().RequestDataType,
                    v => (RequestDataProcessor)Activator.CreateInstance(v));
        }

        public RequestDataProcessor GetProcessorFor(object requestData)
        {
            return _processors[requestData.GetType()];
        }
    }
}
