using BlogSystem.Abstractions;
using BlogSystem.DAL.Context;
using BlogSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystem.Infrastructure
{
    public static class DalDependencyRegistration
    {
        public static IServiceCollection AddBlogSystemDal(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IBlogDbContext>(provider =>
            {
                var options = new DbContextOptionsBuilder<BlogDbContext>()
                    .UseSqlite(connectionString)
                    .UseLazyLoadingProxies()
                    .Options;
                return new BlogDbContext(options);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}