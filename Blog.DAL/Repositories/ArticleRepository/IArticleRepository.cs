using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Entities;

namespace BlogSystem.DAL.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Article>> GetByUserIdAsync(int userId);
        Task<Article> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Article>> GetAllWithDetailsAsync();
    }
}