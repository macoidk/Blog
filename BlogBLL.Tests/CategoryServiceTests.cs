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
    public class CategoryServiceTests : TestBase
    {
        private ICategoryService _categoryService;
        private IUnitOfWork _unitOfWork;
        private Fixture _fixture;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            var categoriesMock = Substitute.For<ICategoryRepository>();
            _unitOfWork.Categories.Returns(categoriesMock);
            
            _categoryService = new CategoryService(_unitOfWork); 
            
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            
        }

        [Test]
        public async Task CreateAsync_ValidCategoryDto_CreatesCategory()
        {
            var categoryDto = _fixture.Build<CategoryDto>().With(c => c.Id, 0).Create();
            _unitOfWork.Categories.AddAsync(Arg.Any<Category>()).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            var result = await _categoryService.CreateAsync(categoryDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(categoryDto.Name));
            await _unitOfWork.Categories.Received(1).AddAsync(Arg.Any<Category>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void CreateAsync_EmptyName_ThrowsServiceException()
        {
            var categoryDto = new CategoryDto { Name = "" };

            Assert.ThrowsAsync<ServiceException>(() => _categoryService.CreateAsync(categoryDto));
        }

        [Test]
        public async Task UpdateAsync_ValidCategoryDto_UpdatesCategory()
        {
            var category = _fixture.Build<Category>().With(c => c.Id, 1).Create();
            var categoryDto = _fixture.Build<CategoryDto>().With(c => c.Id, 1).Create();
            _unitOfWork.Categories.GetByIdAsync(1).Returns(Task.FromResult(category));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _categoryService.UpdateAsync(categoryDto);

            Assert.That(category.Name, Is.EqualTo(categoryDto.Name));
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void UpdateAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            var categoryDto = _fixture.Create<CategoryDto>();
            _unitOfWork.Categories.GetByIdAsync(categoryDto.Id).Returns(Task.FromResult<Category>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _categoryService.UpdateAsync(categoryDto));
        }

        [Test]
        public async Task DeleteAsync_ExistingId_DeletesCategory()
        {
            var category = _fixture.Build<Category>()
                .With(c => c.Id, 1)
                .Without(c => c.Articles)
                .Create();
            _unitOfWork.Categories.GetByIdAsync(1).Returns(Task.FromResult(category));
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(1));

            await _categoryService.DeleteAsync(1);

            _unitOfWork.Categories.Received(1).Delete(category);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Test]
        public void DeleteAsync_NonExistingId_ThrowsEntityNotFoundException()
        {
            _unitOfWork.Categories.GetByIdAsync(999).Returns(Task.FromResult<Category>(null));

            Assert.ThrowsAsync<EntityNotFoundException>(() => _categoryService.DeleteAsync(999));
        }
    }
}