using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RatMQ.Service.DataAccess
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> whereExpression);

        void Save(TEntity entity);

        void Save(IReadOnlyCollection<TEntity> entities);

        void Update(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> whereExpression = null);
    }
}
