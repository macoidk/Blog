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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentDto> GetByIdAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) throw new EntityNotFoundException($"Comment {id} not found");
            return comment.ToDto();
        }

        public async Task<IEnumerable<CommentDto>> GetByArticleIdAsync(int articleId)
        {
            var comments = await _unitOfWork.Comments.GetByArticleIdAsync(articleId);
            return comments.Select(c => c.ToDto());
        }

        public async Task<IEnumerable<CommentDto>> GetRootCommentsByArticleIdAsync(int articleId)
        {
            var comments = await _unitOfWork.Comments.GetRootCommentsByArticleIdAsync(articleId);
            return comments.Select(c => c.ToDto()).ToList();
        }

        public async Task<CommentDto> GetWithRepliesAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetWithRepliesAsync(id);
            if (comment == null) throw new EntityNotFoundException($"Comment {id} not found");
            return comment.ToDto();
        }

        public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId)
        {
            var comments = await _unitOfWork.Comments.GetByUserIdAsync(userId);
            return comments.Select(c => c.ToDto());
        }

        public async Task<CommentDto> CreateAsync(CommentDto commentDto)
        {
            if (string.IsNullOrWhiteSpace(commentDto.Content))
                throw new ArgumentException("Коментар не може бути порожнім.");

            var comment = commentDto.ToEntity();
            comment.CreationDate = DateTime.UtcNow; // Завжди задаємо дату створення
            comment.UpdateDate = null; // Початково null, оновлюється при редагуванні

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            var createdComment = await _unitOfWork.Comments.GetByIdAsync(comment.Id);
            return createdComment.ToDto();
        }

        public async Task UpdateAsync(CommentDto commentDto)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentDto.Id);
            if (comment == null) throw new EntityNotFoundException($"Comment {commentDto.Id} not found");

            comment.Content = commentDto.Content;
            comment.UpdateDate = DateTime.UtcNow;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) throw new EntityNotFoundException($"Comment {id} not found");

            _unitOfWork.Comments.Delete(comment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}