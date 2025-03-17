using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;

namespace BlogSystem.BLL.Interfaces
{
    public interface ITagService
    {
        Task<TagDto> GetByIdAsync(int id);
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto> CreateAsync(string name);
        Task UpdateAsync(TagDto tagDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<TagDto>> GetByArticleIdAsync(int articleId);
        Task<TagDto> GetByNameAsync(string name);
    }
}