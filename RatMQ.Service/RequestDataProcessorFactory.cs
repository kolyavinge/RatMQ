using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RatMQ.Service
{
    class RequestDataProcessorFactory
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
