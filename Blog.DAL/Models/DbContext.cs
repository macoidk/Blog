using BlogSystem.Abstractions;
using BlogSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogSystem.DAL.Context
{
    public class BlogDbContext : DbContext, IBlogDbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public IQueryable<Article> Articles => base.Set<Article>().AsQueryable();
        public IQueryable<Category> Categories => base.Set<Category>().AsQueryable();
        public IQueryable<Comment> Comments => base.Set<Comment>().AsQueryable();
        public IQueryable<Tag> Tags => base.Set<Tag>().AsQueryable();
        public IQueryable<User> Users => base.Set<User>().AsQueryable();
        public IQueryable<ArticleTag> ArticleTags => base.Set<ArticleTag>().AsQueryable();

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        public void Add<TEntity>(TEntity entity) where TEntity : class => base.Add(entity);
        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class => base.AddRange(entities);
        public void Update<TEntity>(TEntity entity) where TEntity : class => base.Update(entity);
        public void Remove<TEntity>(TEntity entity) where TEntity : class => base.Remove(entity);
        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class => base.RemoveRange(entities);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });

            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(at => at.TagId);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.ChildComments)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Технології", Description = "Статті про сучасні технології" },
                new Category { Id = 2, Name = "Наука", Description = "Наукові дослідження та відкриття" },
                new Category { Id = 3, Name = "Подорожі", Description = "Розповіді про подорожі світом" }
            );
        }
    }
}