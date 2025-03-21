using System;
using System.Threading.Tasks;

namespace BlogSystem.Abstractions
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