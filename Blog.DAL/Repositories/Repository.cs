using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly BlogDbContext Context;
        protected readonly DbSet<TEntity> DbSet;
        
        public Repository(BlogDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }
        
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }
        
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }
        
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }
        
        public async Task AddAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }
        
        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }
        
        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}