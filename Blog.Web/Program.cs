using BlogSystem.BLL.Interfaces;
using BlogSystem.BLL.Services;
using BlogSystem.DAL.Context;
using BlogSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using BlogSystem.BLL.Utils;
using BlogSystem.DAL.Context;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сесії
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Налаштування DbContext для SQLite
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies());

// Реєстрація UnitOfWork і сервісів
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}");

app.Run();