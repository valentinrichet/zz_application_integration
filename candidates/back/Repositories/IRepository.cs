using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiCandidate.Repositories
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> Add(TEntity entity);
        Task Delete(TEntity entity);
        ValueTask<TEntity> Get(object primaryKey);
        ValueTask<TEntity> Get(object primaryKeyA, object primaryKeyB);
        IQueryable<TEntity> GetAll();
        Task<TEntity> Update(TEntity entity);
    }
}
