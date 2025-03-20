using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Extensions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.Models;
using BlogSystem.Abstractions;

namespace BlogSystem.BLL.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagService _tagService;

        public ArticleService(IUnitOfWork unitOfWork, ITagService tagService)
        {
            _unitOfWork = unitOfWork;
            _tagService = tagService;
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdWithDetailsAsync(id);
            if (article == null) throw new EntityNotFoundException($"Article {id} not found");
            return article.ToDto();
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            var articles = await _unitOfWork.Articles.GetAllWithDetailsAsync();
            return articles.Select(a => a.ToDto());
        }

        public async Task<IEnumerable<ArticleDto>> GetByCategoryIdAsync(int categoryId)
        {
            var articles = await _unitOfWork.Articles.GetByCategoryIdAsync(categoryId);
            return articles.Select(a => a.ToDto());
        }

        public async Task<IEnumerable<ArticleDto>> GetByUserIdAsync(int userId)
        {
            var articles = await _unitOfWork.Articles.GetByUserIdAsync(userId);
            return articles.Select(a => a.ToDto());
        }

        public async Task<ArticleDto> CreateAsync(ArticleDto articleDto)
        {
            var article = articleDto.ToEntity();
            article.CreationDate = DateTime.UtcNow;

            await _unitOfWork.Articles.AddAsync(article);
            await _unitOfWork.SaveChangesAsync();

            if (articleDto.Tags.Any())
            {
                foreach (var tagDto in articleDto.Tags)
                {
                    await AddTagAsync(article.Id, tagDto.Name);
                }
            }

            return (await _unitOfWork.Articles.GetByIdWithDetailsAsync(article.Id)).ToDto();
        }

        public async Task UpdateAsync(ArticleDto articleDto)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(articleDto.Id);
            if (article == null) throw new EntityNotFoundException($"Article {articleDto.Id} not found");

            article.Title = articleDto.Title;
            article.Content = articleDto.Content;
            article.CategoryId = articleDto.CategoryId;
            article.UpdateDate = DateTime.UtcNow;

            _unitOfWork.Articles.Update(article);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article == null) throw new EntityNotFoundException($"Article {id} not found");

            _unitOfWork.Articles.Delete(article);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddTagAsync(int articleId, int tagId)
        {
            var article = await _unitOfWork.Articles.GetByIdWithDetailsAsync(articleId);
            if (article == null) throw new EntityNotFoundException($"Article {articleId} not found");

            if (!article.ArticleTags.Any(at => at.TagId == tagId))
            {
                article.ArticleTags.Add(new ArticleTag { ArticleId = articleId, TagId = tagId });
                _unitOfWork.Articles.Update(article); 
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task AddTagAsync(int articleId, string tagName)
        {
            var tag = await _tagService.GetByNameAsync(tagName) ?? await _tagService.CreateAsync(tagName);
            await AddTagAsync(articleId, tag.Id);
        }

        public async Task RemoveTagAsync(int articleId, int tagId)
        {
            var article = await _unitOfWork.Articles.GetByIdWithDetailsAsync(articleId);
            if (article == null) throw new EntityNotFoundException($"Article {articleId} not found");

            var articleTag = article.ArticleTags.FirstOrDefault(at => at.TagId == tagId);
            if (articleTag != null)
            {
                article.ArticleTags.Remove(articleTag);
                _unitOfWork.Articles.Update(article); // Оновлюємо статтю після видалення тегу
                await _unitOfWork.SaveChangesAsync();
            }
        }
        
        public async Task<ArticleDto> GetByIdLazyAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdLazyAsync(id);
            if (article == null) throw new EntityNotFoundException($"Article {id} not found");

            var dto = article.ToDto();
            dto.AuthorName = article.User.Username;           
            dto.CommentCount = article.Comments.Count;      
            dto.Tags = article.ArticleTags.Select(at => at.Tag.ToDto()).ToList(); 
            return dto;
        }
        
    }
}