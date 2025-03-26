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
            services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlite(connectionString)
                    .UseLazyLoadingProxies());

            services.AddScoped<IBlogDbContext>(provider => provider.GetRequiredService<BlogDbContext>());
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}