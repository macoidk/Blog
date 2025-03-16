using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Entities;

namespace BlogSystem.DAL.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId);
        Task<IEnumerable<Comment>> GetRootCommentsByArticleIdAsync(int articleId);
        Task<Comment> GetWithRepliesAsync(int id);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
    }
}