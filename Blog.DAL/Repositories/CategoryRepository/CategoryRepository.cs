using BlogSystem.DAL.Context;
using BlogSystem.Models;
using BlogSystem.Abstractions;

namespace BlogSystem.DAL.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogDbContext context) : base(context)
        {
        }
    }
}