using System;
using Microsoft.Extensions.Configuration;

namespace RatMQ.Service.DataAccess
{
    public interface IDataContextLoader
    {
        IDataContext GetDataContext();
    }

    public class DataContextLoader : IDataContextLoader
    {
        private readonly IConfiguration _configuration;

        public DataContextLoader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDataContext GetDataContext()
        {
            var databaseContextType = _configuration.GetValue<string>("DatabaseContextType");
            var initParams = new DataContextInitParams
            {
                DatabasePath = _configuration.GetValue<string>("DatabasePath")
            };
            var dataContext = (IDataContext)Activator.CreateInstance(Type.GetType(databaseContextType));
            dataContext.Init(initParams);

            return dataContext;
        }
    }
}
