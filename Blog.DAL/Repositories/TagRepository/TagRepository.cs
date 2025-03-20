using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.Models;
using BlogSystem.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(BlogDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Tag>> GetByArticleIdAsync(int articleId)
        {
            return await Context.ArticleTags
                .Where(at => at.ArticleId == articleId)
                .Select(at => at.Tag)
                .ToListAsync();
        }

        public async Task<Tag> GetByNameAsync(string name)
        {
            return await FindAsync(t => t.Name == name).ContinueWith(t => t.Result.FirstOrDefault());
        }
    }
}