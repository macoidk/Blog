using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId)
        {
            return await Context.Comments
                .Where(c => c.ArticleId == articleId)
                .Include(c => c.User)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Comment>> GetRootCommentsByArticleIdAsync(int articleId)
        {
            return await Context.Comments
                .Where(c => c.ArticleId == articleId && c.ParentCommentId == null)
                .Include(c => c.User)
                .ToListAsync();
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
        
        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        {
            return await Context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.Article)
                .ToListAsync();
        }
    }
}