using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.Abstractions;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Extensions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.Models;
using NSubstitute;
using NUnit.Framework;
using AutoFixture;
using BlogSystem.BLL.Services;
using Ninject;

namespace BlogSystem.Tests
{
    [TestFixture]
    public class ArticleServiceTests : TestBase
    {
        private IArticleService _articleService;
        private IUnitOfWork _unitOfWork;
        private ITagService _tagService;
        private Fixture _fixture;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _tagService = Substitute.For<ITagService>();
            _articleService = new ArticleService(_unitOfWork, _tagService);
            
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsArticleDto()
        {
            var article = _fixture.Build<Article>().With(a => a.Id, 1).Create();
            _unitOfWork.Articles.GetByIdWithDetailsAsync(1).Returns(Task.FromResult(article));

            var result = await _articleService.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetByIdAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Articles.GetByIdWithDetailsAsync(999).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.GetByIdAsync(999));
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllArticles()
        {
            var articles = _fixture.CreateMany<Article>(3).ToList();
            _unitOfWork.Articles.GetAllWithDetailsAsync().Returns(Task.FromResult(articles.AsEnumerable()));

            var result = await _articleService.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetByCategoryIdAsync_ReturnsArticlesByCategory()
        {
            var articles = _fixture.Build<Article>().With(a => a.CategoryId, 1).CreateMany(2).ToList();
            _unitOfWork.Articles.GetByCategoryIdAsync(1).Returns(Task.FromResult(articles.AsEnumerable()));

            var result = await _articleService.GetByCategoryIdAsync(1);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(a => a.CategoryId == 1), Is.True);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsArticlesByUser()
        {
            var articles = _fixture.Build<Article>().With(a => a.UserId, 1).CreateMany(2).ToList();
            _unitOfWork.Articles.GetByUserIdAsync(1).Returns(Task.FromResult(articles.AsEnumerable()));

            var result = await _articleService.GetByUserIdAsync(1);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(a => a.UserId == 1), Is.True);
        }

        [Test]
        public async Task CreateAsync_ValidArticleDto_CreatesArticle()
        {
            var articleDto = new ArticleDto
            {
                Title = "New Article",
                Content = "Content",
                UserId = 1,
                CategoryId = 1,
                Tags = new List<TagDto> { new TagDto { Id = 1, Name = "Tag1" } }
            };
            var createdArticle = new Article 
            { 
                Id = 99, 
                Title = articleDto.Title, 
                Content = articleDto.Content, 
                UserId = articleDto.UserId, 
                CategoryId = articleDto.CategoryId,
                ArticleTags = new List<ArticleTag> { new ArticleTag { TagId = 1 } }
            };
    
            _unitOfWork.Articles.AddAsync(Arg.Any<Article>()).Returns(Task.CompletedTask).AndDoes(callInfo =>
            {
                var article = callInfo.Arg<Article>();
                article.Id = 99; 
            });
    
            _unitOfWork.Articles.GetByIdWithDetailsAsync(99).Returns(Task.FromResult(createdArticle));
            _tagService.GetByNameAsync("Tag1").Returns(Task.FromResult(new TagDto { Id = 1, Name = "Tag1" }));
    
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1)); // Повертаємо Task<int> зі значенням 1

            var result = await _articleService.CreateAsync(articleDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(99));
            Assert.That(result.Title, Is.EqualTo("New Article"));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task UpdateAsync_ValidArticleDto_UpdatesArticle()
        {
            var article = _fixture.Build<Article>().With(a => a.Id, 1).Create();
            var articleDto = _fixture.Build<ArticleDto>().With(a => a.Id, 1).Create();
            _unitOfWork.Articles.GetByIdAsync(1).Returns(Task.FromResult(article));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _articleService.UpdateAsync(articleDto);

            Assert.That(article.Title, Is.EqualTo(articleDto.Title));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void UpdateAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            var articleDto = _fixture.Create<ArticleDto>();
            _unitOfWork.Articles.GetByIdAsync(articleDto.Id).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.UpdateAsync(articleDto));
        }

        [Test]
        public async Task DeleteAsync_ExistingId_DeletesArticle()
        {
            var article = _fixture.Build<Article>().With(a => a.Id, 1).Create();
            _unitOfWork.Articles.GetByIdAsync(1).Returns(Task.FromResult(article));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _articleService.DeleteAsync(1);

            _unitOfWork.Articles.Received(1).Delete(article);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void DeleteAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Articles.GetByIdAsync(999).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.DeleteAsync(999));
        }

        [Test]
        public async Task AddTagAsync_WithTagId_AddsTagToArticle()
        {
            var articleId = 1;
            var tagId = 2;
    
            var article = new Article 
            { 
                Id = articleId, 
                Title = "Test Article", 
                ArticleTags = new List<ArticleTag>() 
            };
    
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var articleRepository = Substitute.For<IArticleRepository>();
            var tagService = Substitute.For<ITagService>();
    
            unitOfWork.Articles.Returns(articleRepository);
            articleRepository.GetByIdWithDetailsAsync(articleId).Returns(Task.FromResult(article));
    
            var articleService = new ArticleService(unitOfWork, tagService);
    
            await articleService.AddTagAsync(articleId, tagId);
    
            Assert.That(article.ArticleTags.Any(at => at.TagId == tagId), "Tag should be added to article.");
            articleRepository.Received(1).Update(article); // Перевіряємо, що оновлення було викликано
            await unitOfWork.Received(1).SaveChangesAsync(); // Перевіряємо збереження змін
        }

        [Test]
        public void AddTagAsync_WithTagId_NonExistingArticle_ThrowsException()
        {
            _unitOfWork.Articles.GetByIdWithDetailsAsync(999).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.AddTagAsync(999, 2));
        }

        [Test]
        public async Task AddTagAsync_WithTagName_AddsTagToArticle()
        {
            var articleId = 1;
            var tagName = "TestTag";
            var tagDto = new TagDto { Id = 2, Name = tagName };
            var article = new Article 
            { 
                Id = articleId, 
                Title = "Test Article", 
                ArticleTags = new List<ArticleTag>() 
            };
            _unitOfWork.Articles.GetByIdWithDetailsAsync(articleId).Returns(Task.FromResult(article));
            _tagService.GetByNameAsync(tagName).Returns(Task.FromResult(tagDto));

            await _articleService.AddTagAsync(articleId, tagName);

            Assert.That(article.ArticleTags.Any(at => at.TagId == tagDto.Id), "Tag should be added to article.");
            _unitOfWork.Articles.Received(1).Update(article);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public async Task RemoveTagAsync_RemovesTagFromArticle()
        {
            var articleTag = new ArticleTag { ArticleId = 1, TagId = 2 };
            var article = _fixture.Build<Article>()
                .With(a => a.Id, 1)
                .With(a => a.ArticleTags, new List<ArticleTag> { articleTag })
                .Create();
            _unitOfWork.Articles.GetByIdWithDetailsAsync(1).Returns(Task.FromResult(article));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _articleService.RemoveTagAsync(1, 2);

            Assert.That(article.ArticleTags.Any(at => at.TagId == 2), Is.False);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void RemoveTagAsync_NonExistingArticle_ThrowsException()
        {
            _unitOfWork.Articles.GetByIdWithDetailsAsync(999).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.RemoveTagAsync(999, 2));
        }

        [Test]
        public async Task GetByIdLazyAsync_ExistingId_ReturnsArticleDto()
        {
            var article = _fixture.Build<Article>()
                .With(a => a.Id, 1)
                .With(a => a.User, _fixture.Create<User>())
                .With(a => a.Comments, _fixture.CreateMany<Comment>(2).ToList())
                .With(a => a.ArticleTags, _fixture.CreateMany<ArticleTag>(2).ToList())
                .Create();
            foreach (var articleTag in article.ArticleTags)
            {
                articleTag.Tag = _fixture.Create<Tag>();
            }
    
            _unitOfWork.Articles.GetByIdLazyAsync(1).Returns(Task.FromResult(article));
            
            var result = await _articleService.GetByIdLazyAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.CommentCount, Is.EqualTo(2));
            Assert.That(result.Tags.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetByIdLazyAsync_NonExistingId_ThrowsException()
        {
            _unitOfWork.Articles.GetByIdLazyAsync(999).Returns(Task.FromResult<Article>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _articleService.GetByIdLazyAsync(999));
        }
    }
}