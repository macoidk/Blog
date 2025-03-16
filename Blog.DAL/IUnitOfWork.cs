using System;
using System.Threading.Tasks;
using BlogSystem.DAL.Repositories;

namespace BlogSystem.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICategoryRepository Categories { get; }
        IArticleRepository Articles { get; }
        ITagRepository Tags { get; }
        ICommentRepository Comments { get; }
        
        Task<int> SaveChangesAsync();
    }
}