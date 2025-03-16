using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.DAL.Entities;

namespace BlogSystem.DAL.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<IEnumerable<Tag>> GetByArticleIdAsync(int articleId);
        Task<Tag> GetByNameAsync(string name);
    }
}