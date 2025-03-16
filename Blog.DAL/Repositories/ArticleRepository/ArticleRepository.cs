using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(BlogDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId)
        {
            return await Context.Articles
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Article>> GetByUserIdAsync(int userId)
        {
            return await Context.Articles
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
        
        public async Task<Article> GetByIdWithDetailsAsync(int id)
        {
            return await Context.Articles
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Comments)
                .ThenInclude(c => c.User)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        public async Task<IEnumerable<Article>> GetAllWithDetailsAsync()
        {
            return await Context.Articles
                .Include(a => a.User)
                .Include(a => a.Category)
                .Include(a => a.Comments)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .ToListAsync();
        }
    }
}