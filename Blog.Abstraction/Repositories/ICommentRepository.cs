using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.Abstractions
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Comment>> GetRootCommentsByArticleIdAsync(int articleId);
        Task<Comment> GetWithRepliesAsync(int id);
    }
    
}