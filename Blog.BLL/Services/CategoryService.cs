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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new EntityNotFoundException($"Category with id {id} not found");

            return category.ToDto();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => c.ToDto());
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ServiceException("Category name cannot be empty");

            var category = categoryDto.ToEntity();
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category.ToDto();
        }

        public async Task UpdateAsync(CategoryDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ServiceException("Category name cannot be empty");

            var category = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
            if (category == null)
                throw new EntityNotFoundException($"Category with id {categoryDto.Id} not found");

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                throw new EntityNotFoundException($"Category with id {id} not found");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}