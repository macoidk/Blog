using BlogSystem.DAL.Context;
using BlogSystem.DAL.Entities;

namespace BlogSystem.DAL.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogDbContext context) : base(context)
        {
        }
    }
}