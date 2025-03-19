using System.Collections.Generic;
using System.Linq;
using BlogSystem.BLL.DTO;
using BlogSystem.DAL.Entities;

namespace BlogSystem.BLL.Extensions
{
    public static class MappingExtensions
    {
        public static UserDto ToDto(this User entity)
        {
            if (entity == null)
                return null;

            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                Role = entity.Role.ToString(),
                RegistrationDate = entity.RegistrationDate
            };
        }

        public static User ToEntity(this UserDto dto)
        {
            if (dto == null)
                return null;

            return new User
            {
                Id = dto.Id,
                Username = dto.Username,
                Email = dto.Email,
                Role = (UserRole)System.Enum.Parse(typeof(UserRole), dto.Role),
                RegistrationDate = dto.RegistrationDate
            };
        }

        public static CategoryDto ToDto(this Category entity)
        {
            if (entity == null)
                return null;

            return new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        public static Category ToEntity(this CategoryDto dto)
        {
            if (dto == null)
                return null;

            return new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static TagDto ToDto(this Tag entity)
        {
            if (entity == null)
                return null;

            return new TagDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static Tag ToEntity(this TagDto dto)
        {
            if (dto == null)
                return null;

            return new Tag
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        public static ArticleDto ToDto(this Article entity)
        {
            if (entity == null)
                return null;

            return new ArticleDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                CreationDate = entity.CreationDate,
                UpdateDate = entity.UpdateDate,
                UserId = entity.UserId,
                AuthorName = entity.User?.Username ?? "Анонім",
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Name ?? "General",
                Tags = entity.ArticleTags?.Select(at => at.Tag.ToDto()).ToList() ?? new List<TagDto>(),
                CommentCount = entity.Comments?.Count ?? 0
            };
        }

        public static Article ToEntity(this ArticleDto dto)
        {
            if (dto == null)
                return null;

            return new Article
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                CreationDate = dto.CreationDate,
                UpdateDate = dto.UpdateDate,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId
            };
        }

        public static CommentDto ToDto(this Comment entity)
        {
            if (entity == null)
                return null;

            return new CommentDto
            {
                Id = entity.Id,
                Content = entity.Content,
                CreationDate = entity.CreationDate,
                UpdateDate = entity.UpdateDate,
                UserId = entity.UserId,
                AuthorName = entity.User?.Username ?? "Анонім",
                ArticleId = entity.ArticleId,
                ArticleTitle = entity.Article?.Title,
                ParentCommentId = entity.ParentCommentId,
                ChildComments = entity.ChildComments?.Select(c => c.ToDto()).ToList() ?? new List<CommentDto>()
            };
        }

        public static Comment ToEntity(this CommentDto dto)
        {
            if (dto == null)
                return null;

            return new Comment
            {
                Id = dto.Id,
                Content = dto.Content,
                CreationDate = dto.CreationDate,
                UpdateDate = dto.UpdateDate,
                UserId = dto.UserId,
                ArticleId = dto.ArticleId,
                ParentCommentId = dto.ParentCommentId
            };
        }
    }
}