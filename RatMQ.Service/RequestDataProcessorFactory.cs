using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RatMQ.Service
{
    public interface IRequestDataProcessorFactory
    {
        RequestDataProcessor GetProcessorFor(object requestData);
    }

    public class RequestDataProcessorFactory : IRequestDataProcessorFactory
    {
        private Dictionary<Type, RequestDataProcessor> _processors;

        public RequestDataProcessorFactory(IServiceCollection services)
        {
            _processors = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.GetCustomAttributes<RequestDataProcessorAttribute>().Any())
                .ToDictionary(
                    requestDataType => requestDataType.GetCustomAttributes<RequestDataProcessorAttribute>().First().RequestDataType,
                    requestDataProcessorType => CreateInstance(requestDataProcessorType, services));
        }

        public RequestDataProcessor GetProcessorFor(object requestData)
        {
            return _processors[requestData.GetType()];
        }

        private RequestDataProcessor CreateInstance(Type requestDataProcessorType, IServiceCollection services)
        {
            if (requestDataProcessorType.GetConstructors().Count(c => c.GetParameters().Any()) == 1)
            {
                var constructor = requestDataProcessorType.GetConstructors().First(c => c.GetParameters().Any());
                var paramServices = (from param in constructor.GetParameters()
                                     join service in services on param.ParameterType equals service.ServiceType
                                     orderby param.Position
                                     select service).ToList();
                if (paramServices.Count == constructor.GetParameters().Length)
                {
                    var args = paramServices.Select(service => service.ImplementationInstance).ToArray();
                    return (RequestDataProcessor)Activator.CreateInstance(requestDataProcessorType, args);
                }
                else
                {
                    throw new Exception("Cannot create a RequestDataProcessor instance");
                }
            }
            else if (requestDataProcessorType.GetConstructors().Count(c => !c.GetParameters().Any()) == 1)
            {
                return (RequestDataProcessor)Activator.CreateInstance(requestDataProcessorType);
            }
            else
            {
                throw new Exception("Cannot create a RequestDataProcessor instance");
            }
        }
    }
}
