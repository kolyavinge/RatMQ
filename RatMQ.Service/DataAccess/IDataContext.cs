using System;
using System.Collections.Generic;

namespace RatMQ.Service.DataAccess
{
    public interface IDataContext
    {
        void Init(DataContextInitParams initParams);

        IRepository<TEntity> GetRepository<TEntity>();
    }

    public class DataContextInitParams
    {
        public string DatabasePath { get; set; }
    }

    public abstract class DataContext : IDataContext
    {
        private Dictionary<Type, object> _repositories;

        public DataContext()
        {
            _repositories = new Dictionary<Type, object>();
        }

        public abstract void Init(DataContextInitParams initParams);

        public virtual IRepository<TEntity> GetRepository<TEntity>()
        {
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        protected void AddRepository<TEntity>(IRepository<TEntity> repository)
        {
            _repositories.Add(typeof(TEntity), repository);
        }
    }
}
