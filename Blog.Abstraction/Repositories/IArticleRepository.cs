using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.Abstractions
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Article>> GetByUserIdAsync(int userId);
        Task<Article> GetByIdWithDetailsAsync(int id);
        
        Task<Article> GetByIdLazyAsync(int id);
        
        Task<IEnumerable<Article>> GetAllWithDetailsAsync();
    }
}