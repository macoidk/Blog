using System.Collections.Generic;
using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.Abstractions
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<IEnumerable<Tag>> GetByArticleIdAsync(int articleId);
        Task<Tag> GetByNameAsync(string name);
    }
}