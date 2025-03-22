using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.Models;
using BlogSystem.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(IBlogDbContext context, IQueryable<Comment> dbSet) : base(context, dbSet)
        {
        }

        public async Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId)
        {
            return await FindAsync(c => c.ArticleId == articleId);
        }

        public async Task<IEnumerable<Comment>> GetRootCommentsByArticleIdAsync(int articleId)
        {
            return await FindAsync(c => c.ArticleId == articleId && c.ParentCommentId == null);
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        {
            return await FindAsync(c => c.UserId == userId);
        }
        
        public async Task<Comment> GetWithRepliesAsync(int id)
        {
            return await Context.Comments
                .Include(c => c.User)
                .Include(c => c.ChildComments)
                .ThenInclude(c => c.User)
                .Include(c => c.ChildComments)
                .ThenInclude(c => c.ChildComments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        
    }
}