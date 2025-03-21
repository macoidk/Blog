using System.Threading.Tasks;
using BlogSystem.Models;

namespace BlogSystem.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
    }
}