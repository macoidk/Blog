using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;

namespace BlogSystem.BLL.Interfaces
{
    public interface IArticleService
    {
        Task<ArticleDto> GetByIdAsync(int id);
        Task<IEnumerable<ArticleDto>> GetAllAsync();
        Task<IEnumerable<ArticleDto>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ArticleDto>> GetByUserIdAsync(int userId);
        Task<ArticleDto> CreateAsync(ArticleDto articleDto);
        Task UpdateAsync(ArticleDto articleDto);
        Task DeleteAsync(int id);
        Task AddTagAsync(int articleId, int tagId);
        Task AddTagAsync(int articleId, string tagName);
        Task RemoveTagAsync(int articleId, int tagId);
        Task<ArticleDto> GetByIdLazyAsync(int id);
    }
}