using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.Abstractions
{
    public interface IBlogDbContext : IDisposable
    {
        IQueryable<Article> Articles { get; }
        IQueryable<Category> Categories { get; }
        IQueryable<Comment> Comments { get; }
        IQueryable<Tag> Tags { get; }
        IQueryable<User> Users { get; }
        IQueryable<ArticleTag> ArticleTags { get; }

        Task<int> SaveChangesAsync();

        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}