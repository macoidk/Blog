using BlogSystem.Abstractions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.BLL.Services;
using BlogSystem.BLL.Utils; 
using BlogSystem.DAL.UnitOfWork; 
using Ninject.Modules;

namespace BlogSystem.Infrastructure
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<UnitOfWork>();
            Bind<IPasswordHasher>().To<PasswordHasher>();
            Bind<IUserService>().To<UserService>();
            Bind<IArticleService>().To<ArticleService>();
            Bind<ITagService>().To<TagService>();
            Bind<ICommentService>().To<CommentService>();
            Bind<ICategoryService>().To<CategoryService>();
        }
    }
}