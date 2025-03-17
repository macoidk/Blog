using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;

namespace BlogSystem.BLL.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> GetByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId);
        Task<IEnumerable<CommentDto>> GetRootCommentsByArticleIdAsync(int articleId);
        Task<CommentDto> GetWithRepliesAsync(int id);
        Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId);
        Task<CommentDto> CreateAsync(CommentDto commentDto);
        Task UpdateAsync(CommentDto commentDto);
        Task DeleteAsync(int id);
    }
}