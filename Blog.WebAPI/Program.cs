using BlogSystem.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Web.Common.WebHost;
using Ninject.Web.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Налаштування Ninject
var kernel = new StandardKernel();
kernel.Load(new ServiceModule());

// Реєстрація DAL з підключенням до SQLite
builder.Services.AddBlogSystemDal(builder.Configuration.GetConnectionString("DefaultConnection"));

// Додавання сервісів до контейнера
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Налаштування middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();