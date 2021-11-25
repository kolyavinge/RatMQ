using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SimpleDB;

namespace RatMQ.Service.DataAccess.SimpleDB
{
    class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly IDBEngine _engine;

        public Repository(IDBEngine engine)
        {
            _engine = engine;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _engine.GetCollection<TEntity>().Query().Select().ToList();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> whereExpression)
        {
            return _engine.GetCollection<TEntity>().Query().Where(whereExpression).ToList();
        }

        public void Save(TEntity entity)
        {
            _engine.GetCollection<TEntity>().InsertOrUpdate(entity);
        }

        public void Save(IEnumerable<TEntity> entities)
        {
            _engine.GetCollection<TEntity>().InsertOrUpdate(entities);
        }

        public void Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> whereExpression = null)
        {
            _engine.GetCollection<TEntity>().Query().Update(updateExpression, whereExpression);
        }
    }
}
