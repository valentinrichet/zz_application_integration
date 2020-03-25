using AutoMapper;
using AiOffer.Exceptions;
using AiOffer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiOffer.Business
{
    public class Business<TEntity, TRepository> : IBusiness<TEntity>
    where TEntity : class
    where TRepository : IRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        public Business(TRepository repository, IMapper mapper, ILogger<IBusiness<TEntity>> logger)
        {
            this._repository = repository;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<Dto> Add<CreationDto, Dto>(CreationDto creationDto, Func<TEntity, bool> conditionToAdd = null)
        {
            TEntity entity = _mapper.Map<TEntity>(creationDto);

            if (conditionToAdd != null && !conditionToAdd.Invoke(entity))
            {
                throw new ConditionFailedException();
            }

            entity = await _repository.Add(entity);
            Dto mappedEntity = _mapper.Map<Dto>(entity);
            return mappedEntity;
        }

        public async Task Delete(object primaryKey, Func<TEntity, bool> conditionToDelete = null)
        {
            TEntity entity = await _repository.Get(primaryKey);

            if (conditionToDelete != null && !conditionToDelete.Invoke(entity))
            {
                throw new ConditionFailedException();
            }

            await _repository.Delete(entity);
        }

        public async Task Delete(object primaryKeyA, object primaryKeyB = null, Func<TEntity, bool> conditionToDelete = null)
        {
            TEntity entity = await _repository.Get(primaryKeyA, primaryKeyB);

            if (conditionToDelete != null && !conditionToDelete.Invoke(entity))
            {
                throw new ConditionFailedException();
            }

            await _repository.Delete(entity);
        }

        public async Task<ICollection<Dto>> Get<Dto>(Expression<Func<TEntity, bool>> where)
        {
            IQueryable<TEntity> queryable = _repository.GetAll().Where(where);
            ICollection<Dto> mappedEntities = await _mapper.ProjectTo<Dto>(queryable).ToListAsync();
            return mappedEntities;
        }

        public async Task<ICollection<Dto>> GetAll<Dto>()
        {
            IQueryable<TEntity> queryable = _repository.GetAll();
            ICollection<Dto> mappedEntities = await _mapper.ProjectTo<Dto>(queryable).ToListAsync();
            return mappedEntities;
        }

        public Task<Dto> GetFirstOrDefault<Dto>(Expression<Func<TEntity, bool>> where)
        {
            IQueryable<TEntity> queryable = _repository.GetAll().Where(where);
            Dto mappedEntity = _mapper.ProjectTo<Dto>(queryable).FirstOrDefault();
            return Task.FromResult(mappedEntity);
        }

        public async Task<Dto> Update<UpdateDto, Dto>(object primaryKey, UpdateDto updateDto, Func<TEntity, bool> conditionToUpdate = null)
        {
            TEntity entity = await _repository.Get(primaryKey);

            if(conditionToUpdate != null && !conditionToUpdate.Invoke(entity))
            {
                throw new ConditionFailedException();
            }

            _mapper.Map(updateDto, entity);
            entity = await _repository.Update(entity);
            Dto mappedEntity = _mapper.Map<Dto>(entity);
            return mappedEntity;
        }

        public async Task<Dto> Update<UpdateDto, Dto>(object primaryKeyA, object primaryKeyB, UpdateDto updateDto, Func<TEntity, bool> conditionToUpdate = null)
        {
            TEntity entity = await _repository.Get(primaryKeyA, primaryKeyB);

            if (conditionToUpdate != null && !conditionToUpdate.Invoke(entity))
            {
                throw new ConditionFailedException();
            }

            _mapper.Map(updateDto, entity);
            entity = await _repository.Update(entity);
            Dto mappedEntity = _mapper.Map<Dto>(entity);
            return mappedEntity;
        }
    }
}
