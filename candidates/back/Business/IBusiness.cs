using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiCandidate.Business
{
    public interface IBusiness<TEntity>
    {
        Task<Dto> Add<CreationDto, Dto>(CreationDto entityDto, Func<TEntity, bool> conditionToAdd = null);
        Task Delete(object primaryKey, Func<TEntity, bool> conditionToDelete = null);
        Task Delete(object primaryKeyA, object primaryKeyB, Func<TEntity, bool> conditionToDelete = null);
        Task<ICollection<Dto>> Get<Dto>(Expression<Func<TEntity, bool>> where);
        Task<ICollection<Dto>> GetAll<Dto>();
        Task<Dto> GetFirstOrDefault<Dto>(Expression<Func<TEntity, bool>> where);
        Task<Dto> Update<UpdateDto, Dto>(object primaryKey, UpdateDto entityDto, Func<TEntity, bool> conditionToUpdate = null);
        Task<Dto> Update<UpdateDto, Dto>(object primaryKeyA, object primaryKeyB, UpdateDto entityDto, Func<TEntity, bool> conditionToUpdate = null);
    }
}
