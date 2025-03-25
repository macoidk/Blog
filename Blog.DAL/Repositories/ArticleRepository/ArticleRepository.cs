using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.Models;
using BlogSystem.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        public ArticleRepository(IBlogDbContext context, IQueryable<Article> dbSet) : base(context, dbSet)
        {
        }

        public async Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId)
        {
            return await FindAsync(a => a.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Article>> GetByUserIdAsync(int userId)
        {
            return await FindAsync(a => a.UserId == userId);
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
        
        public async Task<Article> GetByIdLazyAsync(int id)
        {
            return await Context.Articles
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
    }
}