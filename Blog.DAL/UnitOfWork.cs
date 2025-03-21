using System;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.Abstractions;
using BlogSystem.DAL.Repositories;

namespace BlogSystem.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlogDbContext _context;
        private IUserRepository _userRepository;
        private ICategoryRepository _categoryRepository;
        private IArticleRepository _articleRepository;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;
        private bool _disposed = false;
        
        public UnitOfWork(BlogDbContext context)
        {
            _context = context;
        }
        
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context);
        public IArticleRepository Articles => _articleRepository ??= new ArticleRepository(_context);
        public ITagRepository Tags => _tagRepository ??= new TagRepository(_context);
        public ICommentRepository Comments => _commentRepository ??= new CommentRepository(_context);
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}