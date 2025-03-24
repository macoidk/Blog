using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BlogSystem.DAL.Context
{
    public class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
    {
        public BlogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();

            string connectionString = "Data Source=blog.db"; // Зміни на свій рядок підключення
            optionsBuilder.UseSqlite(connectionString)
                .UseLazyLoadingProxies();

            return new BlogDbContext(optionsBuilder.Options);
        }
    }
}