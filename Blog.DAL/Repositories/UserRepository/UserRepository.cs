using System.Threading.Tasks;
using BlogSystem.DAL.Context;
using BlogSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext context) : base(context)
        {
        }
        
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        
        public async Task<User> GetByEmailAsync(string email)
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<User> GetByIdWithDetailsAsync(int id)
        {
            return await Context.Users
                .Include(u => u.Articles)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}