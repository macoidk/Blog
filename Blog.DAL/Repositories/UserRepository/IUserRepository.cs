using System.Threading.Tasks;
using BlogSystem.DAL.Entities;

namespace BlogSystem.DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdWithDetailsAsync(int id);
    }
}