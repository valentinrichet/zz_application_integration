using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AiCompany.Repositories.Database
{
    public class DbRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext

    {
        protected readonly TContext _context;
        public DbRepository(TContext context)
        {
            _context = context;
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(TEntity entity)
        {
            if (entity != null)
            {
                try
                {
                    _context.Set<TEntity>().Remove(entity);
                    await _context.SaveChangesAsync();
                } 
                finally
                {
                }
            }
        }

        public async ValueTask<TEntity> Get(object primaryKey)
        {
            TEntity entity = await _context.Set<TEntity>().FindAsync(primaryKey);
            return entity;
        }

        public async ValueTask<TEntity> Get(object primaryKeyA, object primaryKeyB)
        {
            TEntity entity = await _context.Set<TEntity>().FindAsync(primaryKeyA, primaryKeyB);
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            return queryable;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}