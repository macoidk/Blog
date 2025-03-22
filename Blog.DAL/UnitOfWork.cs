using System;
using System.Threading.Tasks;
using BlogSystem.Abstractions;
using BlogSystem.DAL.Repositories;

namespace BlogSystem.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IBlogDbContext _context;
        private IUserRepository _userRepository;
        private ICategoryRepository _categoryRepository;
        private IArticleRepository _articleRepository;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;
        private bool _disposed = false;

        public UnitOfWork(IBlogDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context, _context.Users);
        public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_context, _context.Categories);
        public IArticleRepository Articles => _articleRepository ??= new ArticleRepository(_context, _context.Articles);
        public ITagRepository Tags => _tagRepository ??= new TagRepository(_context, _context.Tags);
        public ICommentRepository Comments => _commentRepository ??= new CommentRepository(_context, _context.Comments);

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