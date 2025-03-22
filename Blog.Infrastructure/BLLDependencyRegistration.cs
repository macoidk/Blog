using BlogSystem.Abstractions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.BLL.Services;
using BlogSystem.BLL.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogSystem.Infrastructure
{
    public static class BLLDependencyRegistration
    {
        public static IServiceCollection AddBlogSystemBLL(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            return services;
        }
    }
}