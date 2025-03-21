using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.Models;
using BlogSystem.Abstractions;

namespace BlogSystem.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await FindAsync(u => u.Username == username).ContinueWith(t => t.Result.FirstOrDefault());
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await FindAsync(u => u.Email == email).ContinueWith(t => t.Result.FirstOrDefault());
        }
    }
}