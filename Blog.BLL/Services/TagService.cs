using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSystem.BLL.DTO;
using BlogSystem.BLL.Exceptions;
using BlogSystem.BLL.Extensions;
using BlogSystem.BLL.Interfaces;
using BlogSystem.DAL.Entities;
using BlogSystem.DAL.UnitOfWork;

namespace BlogSystem.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TagDto> GetByIdAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null)
                throw new EntityNotFoundException($"Tag with id {id} not found");

            return tag.ToDto();
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            return tags.Select(t => t.ToDto());
        }

        public async Task<TagDto> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ServiceException("Tag name cannot be empty");

            var existingTag = await _unitOfWork.Tags.GetByNameAsync(name);
            if (existingTag != null)
                return existingTag.ToDto();

            var tag = new Tag { Name = name };
            await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return tag.ToDto();
        }

        public async Task UpdateAsync(TagDto tagDto)
        {
            if (string.IsNullOrWhiteSpace(tagDto.Name))
                throw new ServiceException("Tag name cannot be empty");

            var tag = await _unitOfWork.Tags.GetByIdAsync(tagDto.Id);
            if (tag == null)
                throw new EntityNotFoundException($"Tag with id {tagDto.Id} not found");

            tag.Name = tagDto.Name;

            _unitOfWork.Tags.Update(tag);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null)
                throw new EntityNotFoundException($"Tag with id {id} not found");

            _unitOfWork.Tags.Delete(tag);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<TagDto>> GetByArticleIdAsync(int articleId)
        {
            var tags = await _unitOfWork.Tags.GetByArticleIdAsync(articleId);
            return tags.Select(t => t.ToDto());
        }

        public async Task<TagDto> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ServiceException("Tag name cannot be empty");

            var tag = await _unitOfWork.Tags.GetByNameAsync(name);
            return tag?.ToDto();
        }
    }
}